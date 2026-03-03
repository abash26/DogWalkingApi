using DogWalkingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogWalkingApi.Controllers;

[ApiController]
[Route("walks/walker")]
[Authorize(Roles = "Walker")]
public class WalkerWalkController : ControllerBase
{
    private readonly IWalkService _walkService;

    public WalkerWalkController(IWalkService walkService)
    {
        _walkService = walkService;
    }

    private int? GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return null;
        if (!int.TryParse(userIdStr, out var userId)) return null;
        return userId;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableWalks()
    {
        var walks = await _walkService.GetPendingWalksAsync();
        return Ok(walks ?? []);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyWalks()
    {
        var walkerId = GetUserId();
        if (walkerId == null) return Unauthorized();

        var walks = await _walkService.GetWalksByWalkerIdAsync(walkerId.Value);
        return Ok(walks ?? []);
    }

    [HttpPut("{id}/accept")]
    public async Task<IActionResult> AcceptWalk(int id)
    {
        var walkerId = GetUserId();
        if (walkerId == null) return Unauthorized();

        await _walkService.AcceptWalkAsync(id, walkerId.Value);
        return NoContent();
    }

    [HttpPut("{id}/start")]
    public async Task<IActionResult> StartWalk(int id)
    {
        var walkerId = GetUserId();
        if (walkerId == null) return Unauthorized();

        await _walkService.StartWalkAsync(id, walkerId.Value);
        return NoContent();
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteWalk(int id)
    {
        var walkerId = GetUserId();
        if (walkerId == null) return Unauthorized();

        await _walkService.CompleteWalkAsync(id, walkerId.Value);
        return NoContent();
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelWalk(int id)
    {
        var walkerId = GetUserId();
        if (walkerId == null) return Unauthorized();

        await _walkService.CancelWalkByWalkerAsync(id, walkerId.Value);
        return NoContent();
    }
}