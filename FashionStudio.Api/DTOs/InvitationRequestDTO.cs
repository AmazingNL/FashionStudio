using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class InvitationRequestDTO
    {
        public int WorkSpaceId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}