using DogWalkingApi.Data;
using DogWalkingApi.Models;

namespace Tests.Helpers;
public class TestHelpers
{
    public static async Task<User> AddTestUser(
        ApplicationDbContext context,
        UserRole role = UserRole.Owner,
        string? email = null,
        string? name = null
    )
    {
        var user = new User
        {
            Email = email ?? $"{Guid.NewGuid()}@test.com",
            Name = name ?? "Walker User",
            Role = role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public static async Task<Dog> AddTestDog(
        ApplicationDbContext context,
        string name = "Buddy",
        int ownerId = 1
    )
    {
        var dog = new Dog
        {
            Name = name,
            Age = 2,
            Size = "Medium",
            OwnerId = ownerId
        };

        context.Dogs.Add(dog);
        await context.SaveChangesAsync();
        return dog;
    }

    public static async Task<Walk> AddTestWalk(
    ApplicationDbContext context,
    int ownerId,
    int? walkerId = null,
    DateTime? startTime = null,
    TimeSpan? duration = null,
    WalkStatus status = WalkStatus.Pending)
    {
        var dog = await AddTestDog(context, ownerId: ownerId);

        // Ensure walker exists
        var walker = await context.Users.FindAsync(walkerId);
        if (walker == null)
        {
            walker = await AddTestUser(context, role: UserRole.Walker);
            walkerId = walker.Id; // override to the new ID
        }

        var walk = new Walk
        {
            DogId = dog.Id,
            OwnerId = ownerId,
            WalkerId = walkerId,
            StartTime = startTime ?? DateTime.UtcNow,
            Duration = duration ?? TimeSpan.FromHours(1),
            Status = status,
            Dog = dog,
            Walker = walker
        };

        context.Walks.Add(walk);
        await context.SaveChangesAsync();

        return walk;
    }
}
