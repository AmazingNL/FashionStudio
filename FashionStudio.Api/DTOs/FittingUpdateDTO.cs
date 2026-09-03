using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class FittingUpdateDTO
    {
        public DateTime? FittingDate { get; set; }
        public string? Notes { get; set; }
        public FittingOutcome? Outcome { get; set; }
    }
}
