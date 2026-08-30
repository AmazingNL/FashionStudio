using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FashionStudio.Api.Interfaces;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Controllers;

namespace FashionStudio.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public AuthController(
        ITokenService tokenService,
        IUserService userService,
        IActivityLogService? activityLogService 
    ) : base(activityLogService)
    {
        _tokenService = tokenService;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        try
        {
            var user = await _userService.VerifyPasswordAsync(request);
            if (user == null)
            {
                return Unauthorized();
            }
            var token = _tokenService.GenerateToken(user);
            //var log = await LogActivityAsync("User", user.Id, "Login");
            return Ok(new 
            { 
                Token = token,
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                Message = "Invalid username or password."
            });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        var user = await _userService.RegisterUserAsync(request);
        return Ok(user);
    }
}
