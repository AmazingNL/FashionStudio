using FashionStudio.Api.Data;
using FashionStudio.Api.Models;
using Microsoft.EntityFrameworkCore;
using FashionStudio.Api.DTOs;
using Mapster;
using FashionStudio.Api.Services.Interfaces;
using FashionStudio.Api.Mappers;


namespace FashionStudio.Api.Services
{
    public class WorkSpaceService : IWorkSpaceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public WorkSpaceService(ApplicationDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }


        public async Task<WorkSpaceResponseDTO> CreateWorkSpaceAsync(WorkSpaceRequestDTO request)
        {
            if (_context == null) throw new InvalidOperationException("Database Context is not available");
            try
            {
                var workSpace = _mapper.Map<WorkSpace>(request);
                workSpace.CreatedAt = DateTime.UtcNow;
                await _context.WorkSpaces.AddAsync(workSpace);
                var user = await _userService.GetUserByIdAsync(request.OwnerId);
                if (user == null) throw new InvalidOperationException("Owner not found");
                user.WorkSpaceId = workSpace.Id;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occurred while creating the workspace.");
            }
        }
        public async Task<WorkSpaceResponseDTO> GetWorkSpaceByIdAsync(int id)
        {
            var entity = await _context.WorkSpaces.FindAsync(id);
            return entity.Adapt<WorkSpaceResponseDTO>();
        }
        public async Task<IEnumerable<WorkSpace>> GetAllWorkSpacesAsync()
        {
            return await _context.WorkSpaces.ToListAsync();
        }
        public async Task<WorkSpace> UpdateWorkSpaceAsync(WorkSpace workSpace)
        {
            _context.Entry(workSpace).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return workSpace;
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