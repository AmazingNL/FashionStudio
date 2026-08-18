using System.ComponentModel.DataAnnotations;

namespace FashionStudio.Api.DTOs
{
    public class WorkSpaceResponseDTO
    {
        public WorkSpaceResponseDTO() { }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int OwnerId { get; set; } = 0;
    }
}