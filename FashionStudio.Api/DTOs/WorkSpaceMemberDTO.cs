using FashionStudio.Api.Models;

namespace FashionStudio.Api.DTOs;

public class WorkSpaceMemberDTO
{
    public int UserId { get; set; } 
    public string FullName { get; set; } = string.Empty;
    public Role? Role { get; set; }
    public DateTime JoinedAt { get; set; }
}