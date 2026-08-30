using Mapster;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Mappers
{
    public class MeasurementMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<MeasurementFieldDTO, MeasurementFiled>()
                .Map(dest => dest.CustomMeasurements, src => src.CustomMeasurements);
        }
    }
}
