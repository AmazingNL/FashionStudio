namespace FashionStudio.Api.DTOs
{
    public class QueryParam
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "JoinedAt";
        public bool IsDescending { get; set; } = true;
    }
}