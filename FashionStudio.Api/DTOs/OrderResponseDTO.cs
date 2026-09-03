using FashionStudio.Api.Attributes;
using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int WorkSpaceId { get; set; }
        public int? AssignedToUserId { get; set; }
        public int CreatedByUserId { get; set; }

        [Searchable]
        public string Title { get; set; } = string.Empty;
        [Searchable]
        public string Description { get; set; } = string.Empty;

        public OrderStatus Status { get; set; }
        public decimal QuotedPrice { get; set; }
        public decimal Discount { get; set; }
        public CurrencyCode Currency { get; set; }
        public DateTime DeadlineDate { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
