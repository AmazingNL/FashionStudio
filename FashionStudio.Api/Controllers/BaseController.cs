using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FashionStudio.Api.Services;

namespace FashionStudio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IActivityLogService _activityLogService;
        public BaseController(IActivityLogService activityLogService )
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

        protected string? GetCurrentUserRole()
        {
            return User.FindFirst(CustomClaimTypes.Role)?.Value;
        }

        protected int? GetCurrentWorkSpaceId()
        {
            var claim = User.FindFirst(CustomClaimTypes.WorkSpaceId);
            return int.TryParse(claim?.Value, out var workspaceId )
                ? workspaceId
                : null;
        }

        protected string? GetCurrentWorkspaceName()
        {
            return User.FindFirst(CustomClaimTypes.WorkspaceName)?.Value;
        }

        protected async Task LogActivityAsync(string entityType, int entityId, string action)
        {
            //    ActivityLogId = activityLogId;
            //    User = user;
            //    WorkSpace = workSpace;
            //    EntityType = entityType;
            //    EntityId = entityId;
            //    Action = action;
            //    Timestamp = timestamp;

            int ? userId = GetCurrentUserId();
            int ? workSpaceId = GetCurrentWorkSpaceId();

            if (userId == null && workSpaceId == null) return;

            ActivityLog log = new
            {
                UserId = userId,
                WorkSpaceId = workSpaceId,
                EntityType = entityType,
                EntityTypeId = entityId,
                Action = action,
                TimeStamp = DateTime.UtcNow,
            };

            await _activityLogService.logActivity(log);

        }
    }
}
