using FashionStudio.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace FashionStudio.Api.Models
{
    public class WorkSpaceMembership
    {
        [Key]
        public int Id { get; set; }
        public int WorkSpaceId { get; set; }
        public WorkSpace? WorkSpace { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public WorkSpaceRole? Role { get; set; } = WorkSpaceRole.Assistant;
    }
}