using FashionStudio.Api.Interfaces;
using FashionStudio.Api.Models;
using Microsoft.EntityFrameworkCore;
using FashionStudio.Api.DTOs;
using MapsterMapper;
using FashionStudio.Api.Data;
using FashionStudio.Api.Exceptions;
namespace FashionStudio.Api.Services
{
    public class WorkSpaceInvitationService : IWorkSpaceInvitation
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IWorkSpaceService _workSpaceService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;


        public WorkSpaceInvitationService(AppDbContext context,
            IUserService userService,
            IWorkSpaceService workSpaceService,
            IEmailService emailService,
            IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _workSpaceService = workSpaceService;
            _emailService = emailService;
            _mapper = mapper;
        }
        public async Task<InvitationResponseDTO> SendInvitationAsync(InvitationRequestDTO invitationRequestDTO,
            int ownerId,
            CancellationToken cancellationToken)
        {
            var workspace = await _context.WorkSpaces.FindAsync(invitationRequestDTO.WorkSpaceId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException("Workspace not found.");
            }

            if (!await _workSpaceService.IsOwnerOfWorkSpaceAsync(ownerId, invitationRequestDTO.WorkSpaceId, cancellationToken))
            {
                throw new UnauthorizedAccessException("Only the workspace owner can send invitations.");
            }
            if (await _workSpaceService.IsMemberOfWorkSpaceAsync(invitationRequestDTO.Email, invitationRequestDTO.WorkSpaceId, cancellationToken))
            {
                throw new ConflictException("User already added as a member");
            }

            var owner = await _context.Users.FindAsync(ownerId, cancellationToken);
            if (owner == null)
            {
                throw new NotFoundException("Owner not found.");
            }
            var existingInvitation = await _context.WorkSpaceInvitations
                .FirstOrDefaultAsync(i => i.Email == invitationRequestDTO.Email
                && i.ExpiresAt > DateTime.UtcNow && i.WorkSpaceId == invitationRequestDTO.WorkSpaceId, cancellationToken: cancellationToken);
            if (existingInvitation != null)
            {
                throw new ConflictException("An invitation has already been sent to this email for the specified workspace.");
            }

            var invitation = _mapper.Map<WorkSpaceInvitation>(invitationRequestDTO);
            invitation.Owner = owner;
            invitation.WorkSpace = workspace;
            await _context.WorkSpaceInvitations.AddAsync(invitation, cancellationToken);
            var emailSent = await _emailService.SendEmailAsync(
                    invitation.Email,
                    invitation.Subject,
                    invitation.Body + " :: " + invitation.InvitationCode,
                    cancellationToken);
            if (!emailSent)
            {
                throw new InvalidOperationException("Failed to send invitation email.");
            }
            await _context.SaveChangesAsync(cancellationToken);
            var invitationDto = _mapper.Map<InvitationResponseDTO>(invitation);
            return invitationDto;
        }

        public async Task<AcceptInvitationDTO> RespondToInvitationAsync(string invitationCode, InvitationStatus accept, CancellationToken cancellationToken)
        {
            var invitation = await _context.WorkSpaceInvitations
                .Include(i => i.WorkSpace)
                .Include(i => i.Owner)
                .FirstOrDefaultAsync(i => i.InvitationCode == invitationCode && i.ExpiresAt > DateTime.UtcNow, cancellationToken: cancellationToken);
            if (invitation == null)
            {
                throw new NotFoundException("Invitation not found or has expired.");
            }

            if (accept == InvitationStatus.Declined)
            {
                _context.WorkSpaceInvitations.Remove(invitation);
                await _context.SaveChangesAsync(cancellationToken);
                return new AcceptInvitationDTO
                {
                    Status = InvitationStatus.Declined
                };
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == invitation.Email, cancellationToken);
            if (user == null)
            {
                // No account for this email yet: leave the invitation pending so RegisterUserAsync
                // can pick it up and finish the membership once they sign up.
                return new AcceptInvitationDTO
                {
                    InvitationCode = invitationCode,
                    Status = InvitationStatus.Accepted,
                    RequiresSignUp = true
                };
            }

            var membership = new WorkSpaceMembership
            {
                User = user,
                WorkSpace = invitation.WorkSpace,
                Role = invitation.Role,
            };
            await _context.WorkSpaceMemberships.AddAsync(membership, cancellationToken);
            _context.WorkSpaceInvitations.Remove(invitation);
            await _context.SaveChangesAsync(cancellationToken);

            return new AcceptInvitationDTO
            {
                Status = InvitationStatus.Accepted
            };
        }

    }
}
