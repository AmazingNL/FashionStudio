using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Interfaces
{
    public interface IWorkSpaceInvitation
    {
        Task<InvitationResponseDTO> SendInvitationAsync(InvitationRequestDTO invitation, int ownerId,CancellationToken cancellationToken);
        Task<AcceptInvitationDTO> RespondToInvitationAsync(string invitationCode, InvitationStatus accept, CancellationToken cancellationToken);

    }
}