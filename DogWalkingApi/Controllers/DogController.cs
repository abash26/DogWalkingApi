using DogWalkingApi.DTOs;
using DogWalkingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogWalkingApi.Controllers;

[ApiController]
[Route("dogs")]
[Authorize]
public class DogController : BaseController
{
    private readonly IDogService _dogService;

    public DogController(IDogService dogService)
    {
        _dogService = dogService;
    }

    [HttpGet]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> GetDogs(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _dogService.GetDogsAsync(userId.Value, page, pageSize);

        return Ok(result);
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
        if (dogDto == null) return BadRequest();

        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var dog = await _dogService.AddDogAsync(dogDto, userId.Value);
        return CreatedAtAction(nameof(GetDogById), new { id = dog.Id }, dog);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> UpdateDog(int id, [FromBody] UpdateDogDTO dogDto)
    {
        if (dogDto == null) return BadRequest();

        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var updatedDog = await _dogService.UpdateDogAsync(id, dogDto, userId.Value);
        if (updatedDog == null) return NotFound();

        return Ok(updatedDog);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteDog(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _dogService.DeleteDogAsync(id, userId.Value);
        if (!result) return NotFound();

        return NoContent();
    }
}