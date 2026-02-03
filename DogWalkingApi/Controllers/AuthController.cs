using DogWalkingApi.DTOs;
using DogWalkingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogWalkingApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        var token = await _authService.LoginAsync(loginDTO);

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("Invalid credentials.");
        }
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        var token = await _authService.RegisterAsync(registerDTO);

        if (string.IsNullOrEmpty(token))
        {
            return Conflict("User already exists.");
        }
        return Ok(new { Token = token });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { Message = "Invalid token" });

        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound(new { Message = "User not found" });

        return Ok(new
        {
            user.Id,
            user.Email
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return Ok(new { Message = "Logout successful." });
    }
}
