namespace DogWalkingApi.Models;

public enum UserRole
{
    Owner = 0,
    Walker = 1
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; } = UserRole.Owner;

    // Navigation
    public ICollection<Dog> Dogs { get; set; } = [];
    public ICollection<Walk> Walks { get; set; } = [];
}
