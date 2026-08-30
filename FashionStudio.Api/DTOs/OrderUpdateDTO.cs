using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class OrderUpdateDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public OrderStatus? Status { get; set; }
        public decimal? QuotedPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Currency { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }
}
