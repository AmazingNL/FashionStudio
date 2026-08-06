using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class Order
    {
        public Order()
        {
        }
        //public Order(int orderId, Customer? customer, WorkSpace? workspace, string title,
        //    string description, OrderStatus status, decimal quotedPrice, decimal discount,
        //    decimal currency, DateTime deadlineDate, DateTime eventDate, DateTime deliveredDate,
        //    User? assignedToUser, User? createdByUser, DateTime createdAt, DateTime updatedAt)
        //{
        //    OrderId = orderId;
        //    Customer = customer;
        //    Workspace = workspace;
        //    Title = title;
        //    Description = description;
        //    Status = status;
        //    QuotedPrice = quotedPrice;
        //    Discount = discount;
        //    Currency = currency;
        //    DeadlineDate = deadlineDate;
        //    EventDate = eventDate;
        //    DeliveredDate = deliveredDate;
        //    AssignedToUser = assignedToUser;
        //    CreatedByUser = createdByUser;
        //    CreatedAt = createdAt;
        //    UpdatedAt = updatedAt;
        //}
        [Key]
        public int Id { get; set; }

        public Customer? Customer { get; set; }
        public int CustomerId { get; set; }

        public WorkSpace? WorkSpace { get; set; }
        public int WorkSpaceId { get; set; }

        public User? AssignedToUser { get; set; }
        public int AssignedToUserId { get; set; }

        public User? CreatedByUser { get; set; }
        public int CreatedByUserId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public decimal QuotedPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Currency { get; set; }
        public DateTime DeadlineDate { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime DeliveredDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
