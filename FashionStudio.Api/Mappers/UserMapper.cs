using Mapster;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;
namespace FashionStudio.Api.Mappers;

public class UserMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequestDTO, User>()
            .Map(dest => dest.JoinedAt, src => DateTime.UtcNow)
            .Map(dest => dest.IsActive, src => true);

        config.NewConfig<User, UserResponseDTO>();
    }
}
