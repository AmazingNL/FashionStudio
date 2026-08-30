using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> CreateOrderAsync(OrderRequestDTO request, int userId, CancellationToken cancellation);
        Task<OrderResponseDTO> GetOrderByIdAsync(int orderId);
        Task<PageResultDTO<OrderResponseDTO>> GetAllOrdersAsync(QueryParam queryParam, CancellationToken cancellation);
        Task<OrderResponseDTO> UpdateOrderAsync(int orderId, OrderUpdateDTO request, int actingUserId, CancellationToken cancellation);
        Task<OrderResponseDTO> AssignOrderToUserAsync(int orderId, int assignedToUserId, int actingUserId, CancellationToken cancellation);
    }
}
