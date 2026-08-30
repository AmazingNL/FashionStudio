namespace FashionStudio.Api.DTOs
{
    public class CustomerRequestDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PreferredContactMethod { get; set; } = string.Empty;
        public string SocialHandle { get; set; } = string.Empty;
        public string Birthday { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
