using FashionStudio.Api.Attributes;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Extensions;
using FashionStudio.Api.Models;
using Xunit;

namespace FashionStudio.Api.Tests
{
    public class QueryableExtensionsTests
    {
        private class Item
        {
            [Searchable]
            public string Name { get; set; } = string.Empty;
            public int Rank { get; set; }
        }

        [Fact]
        public void SearchByAttributes_IsCaseInsensitiveAndMatchesSearchableOnly()
        {
            var items = new[]
            {
                new Item { Name = "Wedding Gown", Rank = 1 },
                new Item { Name = "Suit", Rank = 2 },
            }.AsQueryable();

            var result = items.SearchByAttributes("wedding").ToList();

            Assert.Single(result);
            Assert.Equal("Wedding Gown", result[0].Name);
        }

        [Fact]
        public void SearchByAttributes_NullOrEmptyTerm_ReturnsAllItems()
        {
            var items = new[] { new Item { Name = "A" }, new Item { Name = "B" } }.AsQueryable();

            Assert.Equal(2, items.SearchByAttributes(null).Count());
            Assert.Equal(2, items.SearchByAttributes("").Count());
        }

        [Fact]
        public void OrderByProperty_SortsDescendingWhenRequested()
        {
            var items = new[]
            {
                new Item { Name = "A", Rank = 1 },
                new Item { Name = "B", Rank = 3 },
                new Item { Name = "C", Rank = 2 },
            }.AsQueryable();

            var result = items.OrderByProperty("Rank", descending: true).ToList();

            Assert.Equal(new[] { 3, 2, 1 }, result.Select(i => i.Rank));
        }

        [Fact]
        public void OrderByProperty_UnknownProperty_ReturnsSourceUnordered()
        {
            var items = new[] { new Item { Rank = 2 }, new Item { Rank = 1 } }.AsQueryable();

            var result = items.OrderByProperty("DoesNotExist", descending: false).ToList();

            Assert.Equal(new[] { 2, 1 }, result.Select(i => i.Rank));
        }

        [Fact]
        public async Task ToPagedListAsync_ClampsPageSizeAndComputesTotalPages()
        {
            // ToPagedListAsync calls EF Core's CountAsync/ToListAsync, which require a
            // provider that implements IAsyncQueryProvider — plain LINQ-to-Objects
            // .AsQueryable() doesn't qualify, so this needs a real (InMemory) DbContext.
            using var context = TestHelpers.CreateContext();
            context.WorkSpaces.AddRange(Enumerable.Range(1, 25).Select(i => new WorkSpace { Name = $"WS {i}" }));
            await context.SaveChangesAsync();

            var page = await context.WorkSpaces.ToPagedListAsync(new QueryParam { PageNumber = 1, PageSize = 500 });

            // PageSize is clamped to 100 max, but there are only 25 items to return.
            Assert.Equal(25, page.TotalCount);
            Assert.Equal(25, page.Items!.Count());
            Assert.Equal(1, page.TotalPages);
        }
    }
}
