using FashionStudio.Api.Attributes;

namespace FashionStudio.Api.DTOs
{
    public class CustomerResponseDTO
    {
        public int Id { get; set; }

        [Searchable]
        public string FullName { get; set; } = string.Empty;
        [Searchable]
        public string Phone { get; set; } = string.Empty;
        [Searchable]
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PreferredContactMethod { get; set; } = string.Empty;
        public string SocialHandle { get; set; } = string.Empty;
        public string Birthday { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public int? WorkSpaceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
