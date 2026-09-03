using FashionStudio.Api.Attributes;

namespace FashionStudio.Api.DTOs
{
    public class OrderImageResponseDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int WorkSpaceId { get; set; }
        public int UserId { get; set; }

        [Searchable]
        public string Title { get; set; } = string.Empty;
        [Searchable]
        public string Description { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
