using Mapster;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;
namespace FashionStudio.Api.Mappers;
public class WorkSpaceMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<WorkSpaceRequestDTO, WorkSpace>()
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);

        config.NewConfig<WorkSpace, WorkSpaceResponseDTO>();
    }
}