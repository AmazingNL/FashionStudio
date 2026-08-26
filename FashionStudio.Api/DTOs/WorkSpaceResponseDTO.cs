using FashionStudio.Api.Models;


namespace FashionStudio.Api.DTOs
{
    public class WorkSpaceResponseDTO
    {

        public WorkSpaceResponseDTO() { }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Unit DefaultMeasurementUnit { get; set; } = Unit.Cm;
        public ICollection<WorkSpaceMemberDTO> Memberships { get; set; } = new List<WorkSpaceMemberDTO>();

    }
}