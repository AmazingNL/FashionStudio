using System;
using FashionStudio.Api.Models;
using FashionStudio.Api.Data;
using FashionStudio.Api.Interfaces;


namespace FashionStudio.Api.Services
{
	public class ActivityLog : IActivityLog
    {
		private readonly AppDbContext? _context;

        public ActivityLogService (AppDbContext? context)
		{
			_context = context;
		}
		public async Task<ActivityLog> logActivity(ActivityLog activity)
		{
			try
			{
				_context!.ActivityLogs.Add(activity);
				await _context.SaveChangesAsync();
				return activity;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("An error occurred while logging the activity.", ex);
            }
        }
	}
}
