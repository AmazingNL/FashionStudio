using FashionStudio.Api.Attributes;
using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs
{
    public class PaymentResponseDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int WorkSpaceId { get; set; }
        public int CreatedByUserId { get; set; }

        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTime PaymentDate { get; set; }
        [Searchable]
        public string Notes { get; set; } = string.Empty;
        [Searchable]
        public string ReceiptReference { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
