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
}
