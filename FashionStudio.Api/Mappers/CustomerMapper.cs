using Mapster;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Mappers
{
    public class CustomerMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CustomerRequestDTO, Customer>()
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);

            config.NewConfig<Customer, CustomerResponseDTO>();
        }
    }
}
