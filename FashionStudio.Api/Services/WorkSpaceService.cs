using FashionStudio.Api.Data;
using FashionStudio.Api.Models;
using Microsoft.EntityFrameworkCore;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Interfaces;
using FashionStudio.Api.Mappers;
using MapsterMapper;


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
            if (_context == null) throw new InvalidOperationException("Database Context is not available");
            try
            {
                var workSpace = _mapper.Map<WorkSpace>(request);

                await _context.WorkSpaces.AddAsync(workSpace, cancellation);
                var user = await _userService.GetUserByIdAsync(ownerId);
                if (user == null) throw new InvalidOperationException("Owner not found");
                var membership = new WorkSpaceMembership
                {
                    User = user,
                    WorkSpace = workSpace,
                    Role = Role.Owner,
                };
                await _context.WorkSpaceMemberships.AddAsync(membership, cancellation);
                await _context.SaveChangesAsync(cancellation);

                if (workSpace == null) throw new InvalidOperationException("Workspace not found after creation.");
                return await MapWorkSpaceWithMembersAsync(workSpace);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while creating the workspace.");
            }
        }


        public async Task<bool> IsOwnerOfWorkSpaceAsync(int userId, int workSpaceId, CancellationToken cancellation)
        {
            var membership = await _context.WorkSpaceMemberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.WorkSpaceId == workSpaceId && m.Role == Role.Owner, cancellation);
            return membership != null;
        }

        public async Task<bool> IsMemberOfWorkSpaceAsync(string email, int workSpaceId, CancellationToken cancellation)
        {
            var membership = await _context.WorkSpaceMemberships
                .FirstOrDefaultAsync(m => m.User.Email == email && m.WorkSpaceId == workSpaceId, cancellation);
            return membership != null;
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

        private async Task<WorkSpaceResponseDTO> MapWorkSpaceWithMembersAsync(WorkSpace workSpace)
        {
            var memberships = await _context.WorkSpaceMemberships
                .Include(m => m.User)
                .Where(m => m.WorkSpaceId == workSpace.Id)
                .ToListAsync();
            workSpace.Memberships = memberships;
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
    }
}