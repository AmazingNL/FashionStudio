using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class ActivityLog
    {
        public ActivityLog() { }
        //public ActivityLog(int activityLogId, User? user, WorkSpace? workSpace, object entityType, object entityId, string action, DateTime timestamp)
        //{
        //    ActivityLogId = activityLogId;
        //    User = user;
        //    WorkSpace = workSpace;
        //    EntityType = entityType;
        //    EntityId = entityId;
        //    Action = action;
        //    Timestamp = timestamp;
        //}
        [Key]
        public int Id { get; set; }

        public User? User { get; set; }
        public int UserId { get; set; }

        public WorkSpace? WorkSpace { get; set; }
        public int WorkSpaceId { get; set; }

        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
