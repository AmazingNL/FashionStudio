using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class MeasurementSetDTO
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public Unit Unit { get; set; }
        public DateTime DateTaken { get; set; }
        public ICollection<MeasurementFieldDTO> Fields { get; set; } = new List<MeasurementFieldDTO>();
    }
}
