namespace FashionStudio.Api.DTOs
{
    public class OrderImageUploadDTO
    {
        public int OrderId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }
}
