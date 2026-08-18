using FashionStudio.Api.DTOs;
using FashionStudio.Api.Models;
using MapSter;

namespace FashionStudio.Api.Mappers;
public class WorkSpaceMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<WorkSpace, WorkSpaceResponseDTO>();
    }
}