using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogWalkingApi.Controllers;

[ApiController]
[Route("walks")]
[Authorize]
public class WalkController(IWalkService walkService) : BaseController
{
    private readonly IWalkService _walkService = walkService;

    [HttpGet]
    public async Task<IActionResult> GetWalks()
    {
        var walks = await _walkService.GetWalksAsync();

        return Ok(walks ?? []);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWalkById(int id)
    {
        var walk = await _walkService.GetWalkByIdAsync(id);

        return Ok(walk);
    }

    [HttpGet("walker/{walkerId}")]
    public async Task<IActionResult> GetWalksByWalkerId(int walkerId)
    {
        var walks = await _walkService.GetWalksByWalkerIdAsync(walkerId);

        return Ok(walks ?? []);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetWalksByOwnerId(int ownerId)
    {
        var walks = await _walkService.GetWalksByOwnerIdAsync(ownerId);

        return Ok(walks ?? []);
    }

    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> ScheduleWalk([FromBody] WalkCreateDto walkDto)
    {
        var ownerId = GetUserId();
        if (ownerId == null) return Unauthorized();

        var walk = new Walk
        {
            StartTime = walkDto.StartTime,
            Duration = walkDto.Duration,
            DogId = walkDto.DogId,
            OwnerId = ownerId.Value,
            Status = WalkStatus.Pending
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
