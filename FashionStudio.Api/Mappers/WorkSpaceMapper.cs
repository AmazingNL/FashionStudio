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

        config.NewConfig<Customer, CustomerWithMeasurementsDTO>();

        config.NewConfig<MeasurementSet, MeasurementSetDTO>()
            .Map(dest => dest.Fields, src => src.MeasurementFiled);

        config.NewConfig<MeasurementFiled, MeasurementFieldDTO>()
            .Map(dest => dest.CustomMeasurements, src => src.CustomMeasurements);

    }
}