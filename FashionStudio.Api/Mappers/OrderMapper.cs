using Mapster;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Mappers
{
    public class OrderMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<OrderRequestDTO, Order>()
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);

            config.NewConfig<Order, OrderResponseDTO>();

            config.NewConfig<OrderUpdateDTO, Order>()
                .IgnoreNullValues(true)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);
        }
    }
}
