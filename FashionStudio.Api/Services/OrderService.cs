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
        private readonly IWorkSpaceService _workSpaceService;
        private readonly IOrderImageService _orderImageService;

        public OrderService(AppDbContext context, IMapper mapper, IWorkSpaceService workSpaceService, IOrderImageService orderImageService)
        {
            _context = context;
            _mapper = mapper;
            _workSpaceService = workSpaceService;
            _orderImageService = orderImageService;
        }

        public async Task<OrderResponseDTO> CreateOrderAsync(OrderRequestDTO request, int userId, CancellationToken cancellation)
        {
            await _workSpaceService.EnsureIsOwnerOrAssistantAsync(request.WorkSpaceId, userId, cancellation);

            var customer = await _context.Customers.FindAsync(new object[] { request.CustomerId }, cancellation);
            if (customer == null) throw new NotFoundException("Customer not found");
            if (customer.WorkSpaceId != null && customer.WorkSpaceId != request.WorkSpaceId)
                throw new ConflictException("Customer belongs to a different workspace");

            if (request.AssignedToUserId != null)
            {
                await _workSpaceService.EnsureIsMemberAsync(request.WorkSpaceId, request.AssignedToUserId.Value, cancellation);
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

            await _workSpaceService.EnsureIsMemberAsync(order.WorkSpaceId, actingUserId, cancellation);

            _mapper.Map(request, order);
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<OrderResponseDTO> AssignOrderToUserAsync(int orderId, int assignedToUserId, int actingUserId, CancellationToken cancellation)
        {
            var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellation);
            if (order == null) throw new NotFoundException("Order not found");

            await _workSpaceService.EnsureIsOwnerOrAssistantAsync(order.WorkSpaceId, actingUserId, cancellation);
            await _workSpaceService.EnsureIsMemberAsync(order.WorkSpaceId, assignedToUserId, cancellation);

            order.AssignedToUserId = assignedToUserId;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task DeleteOrderAsync(int orderId, int actingUserId, CancellationToken cancellation)
        {
            var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellation);
            if (order == null) throw new NotFoundException("Order not found");

            await _workSpaceService.EnsureIsOwnerOrAssistantAsync(order.WorkSpaceId, actingUserId, cancellation);

            // Payments are an immutable ledger (see PaymentService) — deleting an order that
            // already has payments recorded against it would silently erase that history via
            // the ON DELETE CASCADE from Orders to Payments, so it's blocked outright.
            var hasPayments = await _context.Payments.AnyAsync(p => p.OrderId == orderId, cancellation);
            if (hasPayments)
                throw new ConflictException("Cannot delete an order that has recorded payments");

            // Fittings/OrderImages cascade-delete at the DB level, but the image *files* on disk
            // don't — clean those up first or they're orphaned forever.
            await _orderImageService.DeleteImagesForOrderAsync(orderId, cancellation);

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync(cancellation);
        }
    }
}
