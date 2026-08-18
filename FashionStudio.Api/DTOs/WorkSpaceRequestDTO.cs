using FashionStudio.Api.Data;
using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class WorkSpaceRequestDTO
    {
        public WorkSpaceRequestDTO() { }

        []
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int OwnerId { get; set; } = 0;
    }
}