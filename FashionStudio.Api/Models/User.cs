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
        public int? WorkSpaceId { get; set; } = null;

        public ICollection<WorkSpace> OwnedWorkSpaces { get; set; } = new List<WorkSpace>();

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; } = true; // true: active, false: inactive
        public string Password { get; internal set; } = string.Empty;

    }
}
