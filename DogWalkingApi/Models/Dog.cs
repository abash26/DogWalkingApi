namespace DogWalkingApi.Models;

public class Dog
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Breed { get; set; }

    public int Age { get; set; }

    public string Size { get; set; }

    public string? SpecialNeeds { get; set; }

    // Foreign Key
    public int OwnerId { get; set; }

    public ICollection<Walk> Walks { get; set; } = [];
}
