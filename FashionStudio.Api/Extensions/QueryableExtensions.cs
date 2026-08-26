using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using FashionStudio.Api.Attributes;
using FashionStudio.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FashionStudio.Api.Extensions;

public static class QueryableExtensions
{
	private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> PropertyCache = new();
	private static readonly ConcurrentDictionary<Type, PropertyInfo[]> SearchablePropertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

	private static readonly MethodInfo StringContainsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;


	public static async Task<PageResultDTO<T>> ToPagedListAsync<T>(
	this IQueryable<T> source,
	QueryParam? queryParam = null,
	CancellationToken cancellationToken = default)
	{
		// STEP 1: Sanitize inputs (Ensure pageNumber >= 1 and pageSize is capped, e.g., max 100)
		queryParam ??= new QueryParam();
		var pageNumber = Math.Max(1, queryParam.PageNumber);
		var pageSize = Math.Clamp(queryParam.PageSize, 1, 100);
		// STEP 2: Get the total count asynchronously from the database
		var totalCount = await source.CountAsync(cancellationToken);
        // STEP 3: Add an optimization short-circuit (If total count is 0, return an empty PagedResult immediately)
		if (totalCount == 0)
		{
			return new PageResultDTO<T>
			{
				Items = null,
				TotalCount = 0,
				PageNumber = pageNumber,
				PageSize = pageSize
			};
		}
		// STEP 4: Apply .Skip() and .Take(), then fetch the items using .ToListAsync()
		var items = await source
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
		// STEP 5: Construct and return your PagedResult<T> object
		return new PageResultDTO<T>
		{
			Items = items,
			TotalCount = totalCount,
			PageNumber = pageNumber,
			PageSize = pageSize
		};
    }

    public static IQueryable<T> OrderByProperty<T>(
    this IQueryable<T> source,
    string? propertyName,
    bool descending = false)
    {
        // STEP 1: Guard clause - if propertyName is null/whitespace, return source untouched
		if (string.IsNullOrEmpty(propertyName)) return source;

		var cleanName = propertyName.Trim().ToLowerInvariant();
		var cacheKey = (typeof(T),  cleanName);
		// STEP 2: Use Reflection to find the PropertyInfo on type T (ignore case!)
		// If property is not found, return source untouched
		var property = PropertyCache.GetOrAdd(cacheKey, key => 
		key.Item1.GetProperties(BindingFlags.Instance | BindingFlags.Public) 
		.FirstOrDefault(p => p.Name.Equals(key.Item2, StringComparison.OrdinalIgnoreCase)));
		if (property == null) return source;
        // STEP 3: Create the Parameter Expression 'x'
		var param = Expression.Parameter(typeof(T), "x");
		// STEP 4: Create the Property Access Expression 'x.PropertyName'
		var propertyAccess = Expression.MakeMemberAccess(param, property);
		// STEP 5: Create the Lambda Expression 'x => x.PropertyName'
		var expression = Expression.Lambda(propertyAccess, param); 
        // STEP 6: Determine method name ("OrderBy" vs "OrderByDescending") and invoke Expression.Call
		string methodName = descending 
			? nameof(Queryable.OrderByDescending) 
			: nameof(Queryable.OrderBy);
		// 7. Dynamically call Queryable.OrderBy<T, TProperty>(source, x => x.PropertyName)
		var resultExpression = Expression.Call(
			typeof(Queryable),
			methodName,
			new Type[] { typeof(T), property.PropertyType },
			source.Expression,
			Expression.Quote(expression));

		// STEP 8: Re-create and return the query using source.Provider.CreateQuery<T>(...)
		return source.Provider.CreateQuery<T>(resultExpression);
    }


    public static IQueryable<T> SearchByAttributes<T>(
        this IQueryable<T> source,
        string? searchTerm)
    {
        // STEP 1: Guard clause - if searchTerm is null, empty, or whitespace, return source
		if (string.IsNullOrEmpty(searchTerm)) return source;
        // STEP 2: Trim the search term
        searchTerm = searchTerm.Trim().ToLowerInvariant();
		// STEP 3: Retrieve searchable string properties for type T (using cached reflection)
		var searchableProps = SearchablePropertiesCache.GetOrAdd(
			typeof(T),
			type => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
			.Where(p => p.PropertyType == typeof(string) &&
					p.GetCustomAttribute<SearchableAttribute>() != null)
					.ToArray()
		);
        // If no searchable properties exist, return source
		if (searchableProps.Length == 0 ) return source;

        // STEP 4: Create Parameter 'x' and Search Term Constant
		var param = Expression.Parameter( typeof( T ), "x" );
		var constant = Expression.Constant( searchTerm);
		Expression? combinedExpression = null;
		// STEP 5: Loop through searchable properties and build:
		foreach (var prop in searchableProps)
		{
			//   - propertyAccess (x.Property)
			var propAccess = Expression.Property(param, prop);
			//   - nullCheck (x.Property != null)
			var nullCheck = Expression.NotEqual(propAccess, Expression.Constant(null, typeof(string)));
			//   - containsCall (x.Property.Contains(searchTerm))
			var containCall = Expression.Call(propAccess, StringContainsMethod, constant);
			//   - propertyExpr = Expression.AndAlso(nullCheck, containsCall)
			var propExpr = Expression.AndAlso(nullCheck, containCall);
			//   - combinedExpr = combinedExpr == null ? propertyExpr : Expression.OrElse(combinedExpression, propertyExpr)
			combinedExpression = combinedExpression == null
				? propExpr : Expression.OrElse(combinedExpression, propExpr);
		}

			// STEP 6: Create Lambda: Expression.Lambda<Func<T, bool>>(combinedExpr, parameter)
		var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression!,  param);

			// STEP 7: Apply to query using source.Where(lambda) and return
		return source.Where(lambda);
    }
}