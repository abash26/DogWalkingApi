using System.ComponentModel.DataAnnotations;

namespace DogWalkingApi.DTOs;

public class CreateDogDTO
{
    [Required]
    public string Name { get; set; }

    public string? Breed { get; set; }

    [Required]
    public int Age { get; set; }

    [Required]
    public string Size { get; set; }

    public string? SpecialNeeds { get; set; }
}
