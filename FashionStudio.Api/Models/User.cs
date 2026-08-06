using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class User
    {
        public User()
        {
        }

        [Key]
        public int Id { get; set; }

        public WorkSpace? WorkSpace { get; set; } = null;
        public int? WorkSpaceId { get; set; }

        public ICollection<WorkSpace> OwnedWorkSpaces { get; set; } = new List<WorkSpace>();

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; } 
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; } = true; // true: active, false: inactive
        public string Password { get; internal set; }
    }
}
