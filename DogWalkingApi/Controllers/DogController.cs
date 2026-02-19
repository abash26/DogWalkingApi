using DogWalkingApi.DTOs;
using DogWalkingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogWalkingApi.Controllers;

[ApiController]
[Route("dogs")]
[Authorize]
public class DogController : ControllerBase
{
    private readonly IDogService _dogService;

    public DogController(IDogService dogService)
    {
        _dogService = dogService;
    }

    private int? GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return null;
        if (!int.TryParse(userIdStr, out var userId)) return null;
        return userId;
    }

    [HttpGet]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> GetDogs()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var dogs = await _dogService.GetDogsAsync(userId.Value);
        if (dogs.Count == 0) return NoContent();

        return Ok(dogs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDogById(int id)
    {
        var dog = await _dogService.GetDogByIdAsync(id);
        if (dog == null) return NotFound();
        return Ok(dog);
    }

    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> CreateDog([FromBody] CreateDogDTO dogDto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var dog = await _dogService.AddDogAsync(dogDto, userId.Value);
        return CreatedAtAction(nameof(GetDogById), new { id = dog.Id }, dog);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> UpdateDog(int id, [FromBody] UpdateDogDTO dogDto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var updatedDog = await _dogService.UpdateDogAsync(id, dogDto, userId.Value);
        if (updatedDog == null) return Forbid();

        return Ok(updatedDog);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteDog(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _dogService.DeleteDogAsync(id, userId.Value);
        if (!result) return Forbid();

        return NoContent();
    }
}
