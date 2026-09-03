using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class Order
    {
        public Order()
        {
        }

        [Key]
        public int Id { get; set; }

        public Customer? Customer { get; set; }
        public int CustomerId { get; set; }

        public WorkSpace? WorkSpace { get; set; }
        public int WorkSpaceId { get; set; }

        public User? AssignedToUser { get; set; }
        public int? AssignedToUserId { get; set; }

        public User? CreatedByUser { get; set; }
        public int CreatedByUserId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public decimal QuotedPrice { get; set; }
        public decimal Discount { get; set; }
        public CurrencyCode Currency { get; set; } = CurrencyCode.NGN;
        public DateTime DeadlineDate { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
