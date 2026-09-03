namespace FashionStudio.Api.DTOs
{
    public class FittingRequestDTO
    {
        public int OrderId { get; set; }
        public DateTime FittingDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
