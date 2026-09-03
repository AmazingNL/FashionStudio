using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Interfaces
{
    public interface IOrderImageService
    {
        Task<OrderImageResponseDTO> UploadImageAsync(OrderImageUploadDTO request, int userId, CancellationToken cancellation);
        Task<OrderImageResponseDTO> GetImageByIdAsync(int imageId, int actingUserId, CancellationToken cancellation);
        Task<(Stream Stream, string ContentType, string FileName)> GetImageFileAsync(int imageId, int actingUserId, CancellationToken cancellation);
        Task<PageResultDTO<OrderImageResponseDTO>> GetAllImagesAsync(QueryParam queryParam, int actingUserId, CancellationToken cancellation);
        Task DeleteImageAsync(int imageId, int actingUserId, CancellationToken cancellation);
        Task DeleteImagesForOrderAsync(int orderId, CancellationToken cancellation);
    }
}
