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

        config.NewConfig<WorkSpaceMembership, WorkSpaceMemberDTO>()
        .Map(dest => dest.FullName, src => src.User!.FullName);

        config.NewConfig<WorkSpaceInvitation, InvitationRequestDTO>();

    }
}