using FashionStudio.Api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models;

    public class WorkSpaceMembership
    {
        [Key]
        public int Id { get; set; }
        public int WorkSpaceId { get; set; }
        public WorkSpace WorkSpace { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public Role? Role { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
