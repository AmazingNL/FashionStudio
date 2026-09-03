using Mapster;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Mappers
{
    public class PaymentMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PaymentRequestDTO, Payment>()
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);

            config.NewConfig<Payment, PaymentResponseDTO>();
        }
    }
}
