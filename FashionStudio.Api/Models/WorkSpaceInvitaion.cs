using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class WorkSpaceInvitation
    {
        public int Id { get; set; }

        // Work space 
        public int WorkSpaceId { get; set; }
        public WorkSpace WorkSpace { get; set; } = null!;

        // User sent the invitation
        public int OwnerId  { get; set; }
        public User Owner { get; set; } = null!;

        public string Email { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;

        public Role? Role { get; set; }

        public string InvitationCode { get; set; } = Guid.NewGuid().ToString();

        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsAccepted { get; set; } = false;
    }
}