using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class Payment
    {
        public Payment()
        {
        }

        //public Payment(int paymentId, WorkSpace? workspace, Order? order, decimal amount,
        //    PaymentMethod method, DateTime paymentDate, string notes,
        //    string receiptReference, User? createdByUser)
        //{
        //    PaymentId = paymentId;
        //    Workspace = workspace;
        //    Order = order;
        //    Amount = amount;
        //    Method = method;
        //    PaymentDate = paymentDate;
        //    Notes = notes;
        //    ReceiptReference = receiptReference;
        //    CreatedByUser = createdByUser;
        //}
        [Key]
        public int Id { get; set; }

        public WorkSpace? WorkSpace { get; set; }
        public int WorkSpaceId { get; set; }

        public Order? Order { get; set; }
        public int OrderId { get; set; }

        public User? CreatedByUser { get; set; }
        public int CreatedByUserId { get; set; }

        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.CreditCard;
        public DateTime PaymentDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string ReceiptReference { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
