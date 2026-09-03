using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class PaymentRequestDTO
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.CreditCard;
        public DateTime PaymentDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string ReceiptReference { get; set; } = string.Empty;
    }
}
