using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FashionStudio.Api.Interfaces;
using FashionStudio.Api.Models;
using System.Threading.Tasks;
using System;

namespace FashionStudio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IActivityLogService? _activityLogService;
        public BaseController(IActivityLogService? activityLogService = null )
        {
            _activityLogService = activityLogService;
        }

        protected int? GetCurrentUserId()
        {
            if (!User.Identity?.IsAuthenticated ?? true) return null;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(claim?.Value, out var userId)
                ? userId
                : null;
        }

        protected async Task<ActivityLog?> LogActivityAsync(string entityType, int entityId, string action)
        {
            if (_activityLogService == null) return null;

            int? currentUserId = GetCurrentUserId();

            var log = new ActivityLog
            {
                EntityType = entityType,
                EntityId = entityId,
                // Fallback to entityId (the user logging in) if currentUserId is null
                UserId = currentUserId ?? entityId,
                Action = action,
                Timestamp = DateTime.UtcNow
            };

            return await _activityLogService.LogActivity(log);
        }
    }
}
