using FashionStudio.Api.Models;
using FashionStudio.Api.Interfaces;
using FashionStudio.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FashionStudio.Api.Controllers;


[ApiController]
[Route("api/user")]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] QueryParam queryParam,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.GetAllUsersAsync(queryParam, cancellationToken);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

}

