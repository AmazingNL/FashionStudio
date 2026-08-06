using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class Fitting
    {
        public Fitting()
        {
        }
        //public Fitting(int fittingId, User? customer, WorkSpace? workSpace,
        //    Order? order, DateTime fittingDate, string notes, User? createdByUser,
        //    DateTime createdAt, DateTime updatedAt, int approved)
        //{
        //    FittingId = fittingId;
        //    Customer = customer;
        //    WorkSpace = workSpace;
        //    Order = order;
        //    FittingDate = fittingDate;
        //    Notes = notes;
        //    CreatedByUser = createdByUser;
        //    CreatedAt = createdAt;
        //    UpdatedAt = updatedAt;
        //    Approved = approved;
        //}
        [Key]
        public int Id { get; set; }

        public User? Customer { get; set; }
        public int CustomerId { get; set; }

        public WorkSpace? WorkSpace { get; set; }
        public int WorkSpaceId { get; set; }

        public Order? Order { get; set; }
        public int OrderId { get; set; }

        public User? CreatedByUser { get; set; }
        public int CreatedByUserId { get; set; }

        public DateTime FittingDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Approved { get; set; } = 0; // 0: pending, 1: approved, -1: rejected
    }
}
