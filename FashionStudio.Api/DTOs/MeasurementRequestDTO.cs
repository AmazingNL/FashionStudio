using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class MeasurementRequestDTO
    {
        public int CustomerId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public Unit Unit { get; set; } = Unit.Cm;
        public DateTime DateTaken { get; set; }
        public MeasurementFieldDTO Fields { get; set; } = new();
    }
}
