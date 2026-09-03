using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;


namespace FashionStudio.Api.Interfaces;

    public interface IWorkSpaceService
    {
        Task<WorkSpaceResponseDTO> CreateWorkSpaceAsync(WorkSpaceRequestDTO request, int ownwerId, CancellationToken cancellation);
        Task<WorkSpaceResponseDTO> GetWorkSpaceByIdAsync(int id);
        Task<IEnumerable<WorkSpaceResponseDTO>> GetAllWorkSpacesAsync();
        Task<bool> IsOwnerOfWorkSpaceAsync(int userId, int workSpaceId, CancellationToken cancellation);
        Task<bool> IsMemberOfWorkSpaceAsync(string email, int workSpaceId, CancellationToken cancellation);
        Task EnsureIsOwnerOrAssistantAsync(int workSpaceId, int userId, CancellationToken cancellation);
        Task EnsureIsMemberAsync(int workSpaceId, int userId, CancellationToken cancellation);
        Task EnsureIsOwnerAsync(int workSpaceId, int userId, CancellationToken cancellation);
        Task<WorkSpaceResponseDTO> UpdateWorkSpaceAsync(int id, WorkSpaceRequestDTO request, CancellationToken cancellation);
        Task<bool> DeleteWorkSpaceAsync(int id);
        Task<WorkSpaceMemberDTO> UpdateMemberRoleAsync(int workSpaceId, int memberUserId, Role newRole, int actingUserId, CancellationToken cancellation);
        Task RemoveMemberAsync(int workSpaceId, int memberUserId, int actingUserId, CancellationToken cancellation);
    }
