using FashionStudio.Api.Models;


namespace FashionStudio.Api.Interfaces;

    public interface IWorkSpaceService
    {
        Task<WorkSpace> CreateWorkSpaceAsync(WorkSpace workSpace);
        Task<WorkSpace> GetWorkSpaceByIdAsync(int id);
        Task<IEnumerable<WorkSpace>> GetAllWorkSpacesAsync();
        Task<WorkSpace> UpdateWorkSpaceAsync(WorkSpace workSpace);
        Task<bool> DeleteWorkSpaceAsync(int id);
    }
