namespace DogWalkingApi.DTOs;

public class CreateDogDTO
{
    public string Name { get; set; }

    public string? Breed { get; set; }

    public int? Age { get; set; }
    public string Size { get; set; }

    public string? SpecialNeeds { get; set; }

    public int OwnerId { get; set; }
}
