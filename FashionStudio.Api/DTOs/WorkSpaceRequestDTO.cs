using System.ComponentModel.DataAnnotations;

namespace FashionStudio.Api.DTOs
{
    public class WorkSpaceRequestDTO
    {
        public WorkSpaceRequestDTO() { }

        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}