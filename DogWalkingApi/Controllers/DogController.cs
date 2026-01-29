using DogWalkingApi.DTOs;
using DogWalkingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DogWalkingApi.Controllers;

[ApiController]
[Route("dogs")]
public class DogController(IDogService dogService) : ControllerBase
{
    private readonly IDogService _dogService = dogService;

    [HttpGet]
    public async Task<IActionResult> GetDogs()
    {
        var dogs = await _dogService.GetDogsAsync();

        if (dogs == null || dogs.Count == 0)
        {
            return NoContent();
        }

        return Ok(dogs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDogById(int id)
    {
        var dog = await _dogService.GetDogByIdAsync(id);
        if (dog == null)
        {
            return NotFound();
        }
        return Ok(dog);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDog([FromBody] CreateDogDTO dog)
    {
        if (dog == null)
        {
            return BadRequest();
        }
        var createdDog = await _dogService.AddDogAsync(dog);
        return CreatedAtAction(nameof(GetDogById), new { id = createdDog.Id }, createdDog);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDog(int id, [FromBody] UpdateDogDTO dog)
    {
        if (dog == null)
        {
            return BadRequest();
        }
        var updatedDog = await _dogService.UpdateDogAsync(id, dog);
        if (updatedDog == null)
        {
            return NotFound();
        }
        return Ok(updatedDog);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDog(int id)
    {
        var result = await _dogService.DeleteDogAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
