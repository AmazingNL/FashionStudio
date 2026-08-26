using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class AcceptInvitationDTO
    {
        public string InvitationCode { get; set; } = string.Empty;
        public InvitationStatus Status { get; set; }
        public bool RequiresSignUp { get; set; }
    }
}
