using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class ActivityLog
    {
        public ActivityLog() { }
        [Key]
        public int Id { get; set; }

        public User? User { get; set; }
        public int? UserId { get; set; }

        public WorkSpace? WorkSpace { get; set; }
        public int? WorkSpaceId { get; set; }

        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
