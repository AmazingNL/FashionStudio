using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> CreateOrderAsync(OrderRequestDTO request, int userId, CancellationToken cancellation);
        Task<OrderResponseDTO> GetOrderByIdAsync(int orderId, int actingUserId, CancellationToken cancellation);
        Task<PageResultDTO<OrderResponseDTO>> GetAllOrdersAsync(QueryParam queryParam, int actingUserId, CancellationToken cancellation);
        Task<OrderResponseDTO> UpdateOrderAsync(int orderId, OrderUpdateDTO request, int actingUserId, CancellationToken cancellation);
        Task<OrderResponseDTO> AssignOrderToUserAsync(int orderId, int assignedToUserId, int actingUserId, CancellationToken cancellation);
        Task DeleteOrderAsync(int orderId, int actingUserId, CancellationToken cancellation);
    }
}
