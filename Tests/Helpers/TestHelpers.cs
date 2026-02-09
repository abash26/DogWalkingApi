using DogWalkingApi.Data;
using DogWalkingApi.Models;

namespace Tests.Helpers;
public class TestHelpers
{
    public static async Task<User> AddTestUser(ApplicationDbContext context, string? email = null, string? name = null)
    {
        var user = new User
        {
            Email = email ?? $"{Guid.NewGuid()}@test.com",
            Name = name ?? "Walker User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public static async Task<Dog> AddTestDog(ApplicationDbContext context, string name = "Buddy", int ownerId = 1)
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

    public static async Task<Walk> AddTestWalk(ApplicationDbContext context, int ownerId,
                                               DateTime? startTime = null, TimeSpan? duration = null,
                                               WalkStatus status = WalkStatus.Scheduled)
    {
        var dog = await AddTestDog(context, ownerId: ownerId);
        var walker = await AddTestUser(context);


        var walk = new Walk
        {
            DogId = dog.Id,
            WalkerId = walker.Id,
            StartTime = startTime ?? DateTime.UtcNow,
            Duration = duration ?? TimeSpan.FromHours(1),
            Status = status
        };

        context.Walks.Add(walk);
        await context.SaveChangesAsync();
        return walk;
    }
}
