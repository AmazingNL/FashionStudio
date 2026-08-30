using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Interfaces
{
    public interface IMeasurementService
    {
        Task<MeasurementSetDTO> CreateMeasurementAsync(MeasurementRequestDTO request, int actingUserId, CancellationToken cancellation);
    }
}
