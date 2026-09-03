using FashionStudio.Api.Attributes;
using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class FittingResponseDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int WorkSpaceId { get; set; }
        public int CustomerId { get; set; }
        public int CreatedByUserId { get; set; }

        public DateTime FittingDate { get; set; }
        [Searchable]
        public string Notes { get; set; } = string.Empty;
        public FittingOutcome Outcome { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
