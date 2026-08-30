namespace FashionStudio.Api.DTOs
{
    public class OrderRequestDTO
    {
        public int CustomerId { get; set; }
        public int WorkSpaceId { get; set; }
        public int? AssignedToUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal QuotedPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Currency { get; set; }
        public DateTime DeadlineDate { get; set; }
        public DateTime EventDate { get; set; }
    }
}
