using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Interfaces
{
    public interface IFittingService
    {
        Task<FittingResponseDTO> CreateFittingAsync(FittingRequestDTO request, int userId, CancellationToken cancellation);
        Task<FittingResponseDTO> GetFittingByIdAsync(int fittingId);
        Task<PageResultDTO<FittingResponseDTO>> GetAllFittingsAsync(QueryParam queryParam, CancellationToken cancellation);
        Task<FittingResponseDTO> UpdateFittingAsync(int fittingId, FittingUpdateDTO request, int actingUserId, CancellationToken cancellation);
        Task DeleteFittingAsync(int fittingId, int actingUserId, CancellationToken cancellation);
    }
}
