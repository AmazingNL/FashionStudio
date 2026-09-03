using FashionStudio.Api.Interfaces;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;
using MapsterMapper;
using Mapster;
using FashionStudio.Api.Data;
using FashionStudio.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using FashionStudio.Api.Exceptions;

namespace FashionStudio.Api.Services
{
    public class FittingService : IFittingService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWorkSpaceService _workSpaceService;

        public FittingService(AppDbContext context, IMapper mapper, IWorkSpaceService workSpaceService)
        {
            _context = context;
            _mapper = mapper;
            _workSpaceService = workSpaceService;
        }

        public async Task<FittingResponseDTO> CreateFittingAsync(FittingRequestDTO request, int userId, CancellationToken cancellation)
        {
            var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellation);
            if (order == null) throw new NotFoundException("Order not found");

            await _workSpaceService.EnsureIsOwnerOrAssistantAsync(order.WorkSpaceId, userId, cancellation);

            var fitting = _mapper.Map<Fitting>(request);
            fitting.WorkSpaceId = order.WorkSpaceId;
            fitting.CustomerId = order.CustomerId;
            fitting.CreatedByUserId = userId;

            await _context.Fittings.AddAsync(fitting, cancellation);
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<FittingResponseDTO>(fitting);
        }

        public async Task<FittingResponseDTO> GetFittingByIdAsync(int fittingId)
        {
            var fitting = await _context.Fittings.FindAsync(fittingId);
            if (fitting == null) throw new NotFoundException("Fitting not found");

            return _mapper.Map<FittingResponseDTO>(fitting);
        }

        public async Task<PageResultDTO<FittingResponseDTO>> GetAllFittingsAsync(QueryParam queryParam, CancellationToken cancellation)
        {
            var pageDto = await _context.Fittings
                .ProjectToType<FittingResponseDTO>()
                .SearchByAttributes(queryParam.SearchTerm)
                .OrderByProperty(queryParam.SortBy, queryParam.IsDescending)
                .ToPagedListAsync(queryParam, cancellation);
            return pageDto;
        }

        public async Task<FittingResponseDTO> UpdateFittingAsync(int fittingId, FittingUpdateDTO request, int actingUserId, CancellationToken cancellation)
        {
            var fitting = await _context.Fittings.FindAsync(new object[] { fittingId }, cancellation);
            if (fitting == null) throw new NotFoundException("Fitting not found");

            await _workSpaceService.EnsureIsOwnerOrAssistantAsync(fitting.WorkSpaceId, actingUserId, cancellation);

            _mapper.Map(request, fitting);
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<FittingResponseDTO>(fitting);
        }

        public async Task DeleteFittingAsync(int fittingId, int actingUserId, CancellationToken cancellation)
        {
            var fitting = await _context.Fittings.FindAsync(new object[] { fittingId }, cancellation);
            if (fitting == null) throw new NotFoundException("Fitting not found");

            await _workSpaceService.EnsureIsOwnerOrAssistantAsync(fitting.WorkSpaceId, actingUserId, cancellation);

            _context.Fittings.Remove(fitting);
            await _context.SaveChangesAsync(cancellation);
        }
    }
}
