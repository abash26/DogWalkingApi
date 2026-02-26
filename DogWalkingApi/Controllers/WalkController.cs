using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogWalkingApi.Controllers;

[ApiController]
[Route("walks")]
[Authorize]
public class WalkController(IWalkService walkService) : ControllerBase
{
    private readonly IWalkService _walkService = walkService;

    private int? GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return null;
        if (!int.TryParse(userIdStr, out var userId)) return null;
        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> GetWalks()
    {
        var walks = await _walkService.GetWalksAsync();

        if (walks == null) return NoContent();

        return Ok(walks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWalkById(int id)
    {
        var walk = await _walkService.GetWalkByIdAsync(id);

        if (walk == null) return NotFound();

        return Ok(walk);
    }

    // Get all walks for a walker
    [HttpGet("walker/{walkerId}")]
    public async Task<IActionResult> GetWalksByWalkerId(int walkerId)
    {
        var walks = await _walkService.GetWalksByWalkerIdAsync(walkerId);

        if (walks == null || walks.Count == 0) return NoContent();

        return Ok(walks);
    }

    // Get all walks for an owner
    [HttpGet("owner/{ownerId}")]
    public async Task<IActionResult> GetWalksByOwnerId(int ownerId)
    {
        var walks = await _walkService.GetWalksByOwnerIdAsync(ownerId);

        if (walks == null || walks.Count == 0) return NoContent();

        return Ok(walks);
    }

    // Schedule a new walk
    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> ScheduleWalk([FromBody] WalkCreateDto walkDto)
    {
        var walk = new Walk
        {
            StartTime = walkDto.StartTime,
            Duration = walkDto.Duration,
            DogId = walkDto.DogId,
            WalkerId = walkDto.WalkerId,
            Status = WalkStatus.Scheduled
        };

        var scheduled = await _walkService.ScheduleWalkAsync(walk);

        return CreatedAtAction(nameof(GetWalkById), new { id = scheduled.Id }, scheduled);
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelWalk(int id)
    {
        var ownerId = GetUserId();
        if (ownerId == null) return Unauthorized();

        await _walkService.CancelWalkByOwnerAsync(id, ownerId.Value);

        return NoContent();
    }
}
