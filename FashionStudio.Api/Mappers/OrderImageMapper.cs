using Mapster;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Mappers
{
    public class OrderImageMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // DownloadUrl is filled in by OrderImageService after mapping, not here —
            // ProjectToType runs this config as a SQL projection for GetAllImagesAsync,
            // and string interpolation isn't guaranteed to translate there.
            config.NewConfig<OrderImage, OrderImageResponseDTO>();
        }
    }
}
