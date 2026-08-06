using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class Customer
    {
        public Customer() { }
        //public Customer(int Id, WorkSpace? workspace, string fullName,
        //    string phone, string email, string address, string preferredContactMethod,
        //    string socialHandle, string birthday, string notes, User? createdByUser,
        //    DateTime createdAt, DateTime updatedAt)
        //{
        //    this.Id = Id;
        //    Workspace = workspace;
        //    FullName = fullName;
        //    Phone = phone;
        //    Email = email;
        //    Address = address;
        //    PreferredContactMethod = preferredContactMethod;
        //    SocialHandle = socialHandle;
        //    Birthday = birthday;
        //    Notes = notes;
        //    CreatedByUser = createdByUser;
        //    CreatedAt = createdAt;
        //    UpdatedAt = updatedAt;
        //}
        [Key]
        public int Id { get; set; }

        public WorkSpace? WorkSpace { get; set; }
        public int WorkSpaceId { get; set; }

        public User? CreatedByUser { get; set; }
        public int CreatedByUserId { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PreferredContactMethod { get; set; } = string.Empty;
        public string SocialHandle { get; set; } = string.Empty;
        public string Birthday { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
