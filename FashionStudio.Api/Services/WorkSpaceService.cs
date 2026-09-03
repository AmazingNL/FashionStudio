using FashionStudio.Api.Data;
using FashionStudio.Api.Models;
using Microsoft.EntityFrameworkCore;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Interfaces;
using MapsterMapper;
using FashionStudio.Api.Exceptions;


namespace FashionStudio.Api.Services
{
    public class WorkSpaceService : IWorkSpaceService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public WorkSpaceService(AppDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }


        public async Task<WorkSpaceResponseDTO> CreateWorkSpaceAsync(
            WorkSpaceRequestDTO request, int ownerId, CancellationToken cancellation)
        {
            var workSpace = _mapper.Map<WorkSpace>(request);

            var existingWorkSpace = _context.WorkSpaces.FirstOrDefault(w => w.Name == workSpace.Name);
            if (existingWorkSpace != null) throw new ConflictException("Work Space already exist"); 

            await _context.WorkSpaces.AddAsync(workSpace, cancellation);
            var user = await _userService.GetUserByIdAsync(ownerId);
            if (user == null) throw new NotFoundException("Owner not found");
            var membership = new WorkSpaceMembership
            {
                User = user,
                WorkSpace = workSpace,
                Role = Role.Owner,
            };
            await _context.WorkSpaceMemberships.AddAsync(membership, cancellation);
            await _context.SaveChangesAsync(cancellation);

            if (workSpace == null) throw new NotFoundException("Workspace not found after creation.");
            return await MapWorkSpaceWithMembersAsync(workSpace);
        }


        public async Task<bool> IsOwnerOfWorkSpaceAsync(
            int userId, 
            int workSpaceId, 
            CancellationToken cancellation)
        {
            var membership = await _context.WorkSpaceMemberships
                .FirstOrDefaultAsync(m => m.UserId == userId 
                && m.WorkSpaceId == workSpaceId 
                && m.Role == Role.Owner, cancellation);
            return membership != null;
        }

        public async Task<bool> IsMemberOfWorkSpaceAsync(
            string email,
            int workSpaceId,
            CancellationToken cancellation)
        {
            var membership = await _context.WorkSpaceMemberships
                .FirstOrDefaultAsync(m => m.User.Email == email
                && m.WorkSpaceId == workSpaceId, cancellation);
            return membership != null;
        }

        public async Task EnsureIsOwnerOrAssistantAsync(int workSpaceId, int userId, CancellationToken cancellation)
        {
            var membership = await _context.WorkSpaceMemberships
                .FirstOrDefaultAsync(m => m.WorkSpaceId == workSpaceId && m.UserId == userId, cancellation);
            if (membership == null || (membership.Role != Role.Owner && membership.Role != Role.Assistant))
                throw new UnauthorizedAccessException("User must be an Owner or Assistant of this workspace");
        }

        public async Task EnsureIsMemberAsync(int workSpaceId, int userId, CancellationToken cancellation)
        {
            var membership = await _context.WorkSpaceMemberships
                .FirstOrDefaultAsync(m => m.WorkSpaceId == workSpaceId && m.UserId == userId, cancellation);
            if (membership == null)
                throw new NotFoundException("User is not a member of this workspace");
        }

        public async Task EnsureIsOwnerAsync(int workSpaceId, int userId, CancellationToken cancellation)
        {
            var membership = await _context.WorkSpaceMemberships
                .FirstOrDefaultAsync(m => m.WorkSpaceId == workSpaceId && m.UserId == userId, cancellation);
            if (membership == null || membership.Role != Role.Owner)
                throw new UnauthorizedAccessException("Only the workspace owner can perform this action");
        }


        public async Task<WorkSpaceResponseDTO> GetWorkSpaceByIdAsync(int id)
        {
            var workSpace = await _context.WorkSpaces.FindAsync(id);
            if (workSpace == null) throw new InvalidOperationException("Workspace not found");
            return await MapWorkSpaceWithMembersAsync(workSpace);
        }

        public async Task<IEnumerable<WorkSpaceResponseDTO>> GetAllWorkSpacesAsync()
        {
            var workSpaces = await _context.WorkSpaces
                .Include(ws => ws.Memberships)
                .ThenInclude(m => m.User)
                .ToListAsync();
            return _mapper.Map<IEnumerable<WorkSpaceResponseDTO>>(workSpaces);
        }

        // Helper methods
        private async Task<WorkSpaceResponseDTO> MapWorkSpaceWithMembersAsync(WorkSpace workSpace)
        {
            var memberships = await _context.WorkSpaceMemberships
                .Include(m => m.User)
                .Where(m => m.WorkSpaceId == workSpace.Id)
                .ToListAsync();
            workSpace.Memberships = memberships;

            var customers = await _context.Customers
                .Include(c => c.MeasurementSets)
                    .ThenInclude(ms => ms.MeasurementFiled)
                .Where(c => c.WorkSpaceId == workSpace.Id)
                .ToListAsync();
            workSpace.Customers = customers;

            return _mapper.Map<WorkSpaceResponseDTO>(workSpace);
        }

        public async Task<WorkSpaceResponseDTO> UpdateWorkSpaceAsync(int id, WorkSpaceRequestDTO request, CancellationToken cancellation)
        {
            var workSpace = await _context.WorkSpaces.FindAsync(id);
            if (workSpace == null) throw new InvalidOperationException("Workspace not found");
            _mapper.Map(request, workSpace);
            _context.Entry(workSpace).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return _mapper.Map<WorkSpaceResponseDTO>(workSpace);
        }
        public async Task<bool> DeleteWorkSpaceAsync(int id)
        {
            var workSpace = await _context.WorkSpaces.FindAsync(id);
            if (workSpace == null)
            {
                return false;
            }
            _context.WorkSpaces.Remove(workSpace);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<WorkSpaceMemberDTO> UpdateMemberRoleAsync(int workSpaceId, int memberUserId, Role newRole, int actingUserId, CancellationToken cancellation)
        {
            await EnsureIsOwnerAsync(workSpaceId, actingUserId, cancellation);

            var membership = await _context.WorkSpaceMemberships
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.WorkSpaceId == workSpaceId && m.UserId == memberUserId, cancellation);
            if (membership == null) throw new NotFoundException("Membership not found");

            if (membership.Role == Role.Owner && newRole != Role.Owner)
            {
                await EnsureNotLastOwnerAsync(workSpaceId, cancellation);
            }

            membership.Role = newRole;
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<WorkSpaceMemberDTO>(membership);
        }

        public async Task RemoveMemberAsync(int workSpaceId, int memberUserId, int actingUserId, CancellationToken cancellation)
        {
            await EnsureIsOwnerAsync(workSpaceId, actingUserId, cancellation);

            var membership = await _context.WorkSpaceMemberships
                .FirstOrDefaultAsync(m => m.WorkSpaceId == workSpaceId && m.UserId == memberUserId, cancellation);
            if (membership == null) throw new NotFoundException("Membership not found");

            if (membership.Role == Role.Owner)
            {
                await EnsureNotLastOwnerAsync(workSpaceId, cancellation);
            }

            // Removes only the membership row — the underlying User account is never touched.
            _context.WorkSpaceMemberships.Remove(membership);
            await _context.SaveChangesAsync(cancellation);
        }

        private async Task EnsureNotLastOwnerAsync(int workSpaceId, CancellationToken cancellation)
        {
            var ownerCount = await _context.WorkSpaceMemberships
                .CountAsync(m => m.WorkSpaceId == workSpaceId && m.Role == Role.Owner, cancellation);
            if (ownerCount <= 1)
                throw new ConflictException("A workspace must always have at least one owner");
        }
    }
}