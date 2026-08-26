using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FashionStudio.Api.Interfaces;
using FashionStudio.Api.Models;
using System.Threading.Tasks;
using System;
using FashionStudio.Api.Constants;

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

        protected string? GetCurrentUserRole()
        {
            return User.FindFirst(CustomClaimTypes.Role)?.Value;
        }

        protected int? GetCurrentWorkSpaceId()
        {
            var claim = User.FindFirst(CustomClaimTypes.WorkspaceId);
            return int.TryParse(claim?.Value, out var workspaceId )
                ? workspaceId
                : null;
        }

        protected string? GetCurrentWorkspaceName()
        {
            return User.FindFirst(CustomClaimTypes.WorkspaceName)?.Value;
        }

        protected async Task<ActivityLog?> LogActivityAsync(string entityType, int entityId, string action)
        {
            if (_activityLogService == null) return null;

            int? currentUserId = GetCurrentUserId();
            int? currentWorkspaceId = GetCurrentWorkSpaceId();

            var log = new ActivityLog
            {
                EntityType = entityType,
                EntityId = entityId,
                // Fallback to entityId (the user logging in) if currentUserId is null
                UserId = currentUserId ?? entityId,
                WorkSpaceId = currentWorkspaceId, // Assign nullable directly (do not use .Value)
                Action = action,
                Timestamp = DateTime.UtcNow
            };

            return await _activityLogService.LogActivity(log);
        }
    }
}
