using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogWalkingApi.Controllers;

[ApiController]
[Route("walks/owner")]
[Authorize(Roles = "Owner")]
public class WalkController : BaseController
{
    private readonly IWalkService _walkService;

    public WalkController(IWalkService walkService)
    {
        _walkService = walkService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWalkById(int id)
    {
        var walk = await _walkService.GetWalkByIdAsync(id);

        return Ok(walk);
    }

    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetWalkStatus(int id)
    {
        var walk = await _walkService.GetWalkByIdAsync(id);

        if (walk == null)
            return NotFound();

        return Ok(new
        {
            walk.Id,
            Status = walk.Status.ToString()
        });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetWalksByOwnerId(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        var ownerId = GetUserId();
        if (ownerId == null) return Unauthorized();

        var result = await _walkService.GetWalksByOwnerIdAsync(
            ownerId.Value, page, pageSize);

        return Ok(result);
    }

    [HttpPost]
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
