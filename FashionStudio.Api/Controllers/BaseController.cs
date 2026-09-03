using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FashionStudio.Api.Interfaces;
using FashionStudio.Api.Models;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FashionStudio.Api.Controllers
{
    // No [ApiController]/[Route] here — this class is abstract and never routed to directly.
    // Having a route here too (api/[controller], resolving to e.g. "api/WorkSpace") registered
    // a second, conflicting route template alongside each derived controller's own explicit
    // [Route("api/workspace")], which made CreatedAtAction's link generation fail with
    // "No route matches the supplied values" on every create-style endpoint that used it.
    public abstract class BaseController : ControllerBase
    {
        protected readonly IActivityLogService? _activityLogService;
        public BaseController(IActivityLogService? activityLogService = null)
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

            try
            {
                return await _activityLogService.LogActivity(log);
            }
            catch (Exception ex)
            {
                // Audit logging is best-effort: a failure here must never fail the
                // request for the business operation that already succeeded.
                var logger = HttpContext.RequestServices.GetService<ILogger<BaseController>>();
                logger?.LogError(ex, "Failed to record activity log for {EntityType} {EntityId} ({Action})", entityType, entityId, action);
                return null;
            }
        }
    }
}
