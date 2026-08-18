using FashionStudio.Api.Attributes;

namespace FashionStudio.Api.DTOs;

public class UserResponseDTO
{
    [Searchable]
    public string WorkSpace { get; set; } = string.Empty;
    [Searchable]
    public string FirstName { get; set; } = string.Empty;
    [Searchable]
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    [Searchable]
    public string UserName { get; set; } = string.Empty;
    [Searchable]
    public string Email { get; set; } = string.Empty;
    [Searchable]
    public string Role { get; set; } = string.Empty;
    [Searchable]
    public string JoinedAt { get; set; } = string.Empty;

}
