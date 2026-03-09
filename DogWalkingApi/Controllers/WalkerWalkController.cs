using DogWalkingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogWalkingApi.Controllers;

[ApiController]
[Route("walks/walker")]
[Authorize(Roles = "Walker")]
public class WalkerWalkController : BaseController
{
    private readonly IWalkService _walkService;

    public WalkerWalkController(IWalkService walkService)
    {
        _walkService = walkService;
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

        try
        {
            await _walkService.AcceptWalkAsync(id, walkerId.Value);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return Conflict("Walk already accepted.");
        }
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