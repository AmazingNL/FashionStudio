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
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderResponseDTO> CreateOrderAsync(OrderRequestDTO request, int userId, CancellationToken cancellation)
        {
            await EnsureIsOwnerOrAssistantAsync(request.WorkSpaceId, userId, cancellation);

            var customer = await _context.Customers.FindAsync(new object[] { request.CustomerId }, cancellation);
            if (customer == null) throw new NotFoundException("Customer not found");
            if (customer.WorkSpaceId != null && customer.WorkSpaceId != request.WorkSpaceId)
                throw new ConflictException("Customer belongs to a different workspace");

            if (request.AssignedToUserId != null)
            {
                await EnsureIsMemberAsync(request.WorkSpaceId, request.AssignedToUserId.Value, cancellation);
            }

            var order = _mapper.Map<Order>(request);
            order.CreatedByUserId = userId;

            await _context.Orders.AddAsync(order, cancellation);
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<OrderResponseDTO> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw new NotFoundException("Order not found");

            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<PageResultDTO<OrderResponseDTO>> GetAllOrdersAsync(QueryParam queryParam, CancellationToken cancellation)
        {
            var pageDto = await _context.Orders
                .ProjectToType<OrderResponseDTO>()
                .SearchByAttributes(queryParam.SearchTerm)
                .OrderByProperty(queryParam.SortBy, queryParam.IsDescending)
                .ToPagedListAsync(queryParam, cancellation);
            return pageDto;
        }

        public async Task<OrderResponseDTO> UpdateOrderAsync(int orderId, OrderUpdateDTO request, int actingUserId, CancellationToken cancellation)
        {
            var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellation);
            if (order == null) throw new NotFoundException("Order not found");

            await EnsureIsMemberAsync(order.WorkSpaceId, actingUserId, cancellation);

            _mapper.Map(request, order);
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<OrderResponseDTO> AssignOrderToUserAsync(int orderId, int assignedToUserId, int actingUserId, CancellationToken cancellation)
        {
            var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellation);
            if (order == null) throw new NotFoundException("Order not found");

            await EnsureIsOwnerOrAssistantAsync(order.WorkSpaceId, actingUserId, cancellation);
            await EnsureIsMemberAsync(order.WorkSpaceId, assignedToUserId, cancellation);

            order.AssignedToUserId = assignedToUserId;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<OrderResponseDTO>(order);
        }

        // Helper methods
        private async Task EnsureIsOwnerOrAssistantAsync(int workSpaceId, int userId, CancellationToken cancellation)
        {
            var membership = await _context.WorkSpaceMemberships
                .FirstOrDefaultAsync(m => m.WorkSpaceId == workSpaceId && m.UserId == userId, cancellation);
            if (membership == null || (membership.Role != Role.Owner && membership.Role != Role.Assistant))
                throw new UnauthorizedAccessException("User must be an Owner or Assistant of this workspace");
        }

        private async Task EnsureIsMemberAsync(int workSpaceId, int userId, CancellationToken cancellation)
        {
            var membership = await _context.WorkSpaceMemberships
                .FirstOrDefaultAsync(m => m.WorkSpaceId == workSpaceId && m.UserId == userId, cancellation);
            if (membership == null)
                throw new NotFoundException("User is not a member of this workspace");
        }
    }
}
