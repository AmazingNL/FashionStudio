using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Interfaces
{
    public interface IOrderImageService
    {
        Task<OrderImageResponseDTO> UploadImageAsync(OrderImageUploadDTO request, int userId, CancellationToken cancellation);
        Task<OrderImageResponseDTO> GetImageByIdAsync(int imageId);
        Task<(Stream Stream, string ContentType, string FileName)> GetImageFileAsync(int imageId, CancellationToken cancellation);
        Task<PageResultDTO<OrderImageResponseDTO>> GetAllImagesAsync(QueryParam queryParam, CancellationToken cancellation);
        Task DeleteImageAsync(int imageId, int actingUserId, CancellationToken cancellation);
        Task DeleteImagesForOrderAsync(int orderId, CancellationToken cancellation);
    }
}
