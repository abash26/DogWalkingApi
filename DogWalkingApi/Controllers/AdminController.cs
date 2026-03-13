using DogWalkingApi.Models;
using DogWalkingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogWalkingApi.Controllers;

[ApiController]
[Route("admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IWalkService _walkService;
    private readonly IDogService _dogService;

    public AdminController(
        IUserService userService,
        IWalkService walkService,
        IDogService dogService)
    {
        _userService = userService;
        _walkService = walkService;
        _dogService = dogService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UserRole role)
    {
        var result = await _userService.UpdateUserRoleAsync(id, role);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("walks")]
    public async Task<IActionResult> GetAllWalks()
    {
        var walks = await _walkService.GetWalksAsync();
        return Ok(walks);
    }

    [HttpPut("walks/{id}/cancel")]
    public async Task<IActionResult> CancelWalk(int id)
    {
        await _walkService.CancelWalk(id);
        return NoContent();
    }

    [HttpGet("dogs")]
    public async Task<IActionResult> GetAllDogs()
    {
        var dogs = await _dogService.GetAllDogsAsync();
        return Ok(dogs);
    }
}