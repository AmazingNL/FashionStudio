using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> CreatePaymentAsync(PaymentRequestDTO request, int userId, CancellationToken cancellation);
        Task<PaymentResponseDTO> GetPaymentByIdAsync(int paymentId, int actingUserId, CancellationToken cancellation);
        Task<PageResultDTO<PaymentResponseDTO>> GetAllPaymentsAsync(QueryParam queryParam, int actingUserId, CancellationToken cancellation);
    }
}
