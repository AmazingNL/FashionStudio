using Mapster;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Mappers
{
    public class FittingMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<FittingRequestDTO, Fitting>()
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);

            config.NewConfig<Fitting, FittingResponseDTO>();

            config.NewConfig<FittingUpdateDTO, Fitting>()
                .IgnoreNullValues(true)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);
        }
    }
}
