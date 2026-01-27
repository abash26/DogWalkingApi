using DogWalkingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DogWalkingApi.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Dog> Dogs { get; set; }
    public DbSet<Walk> Walks { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Alice", Email = "alice@example.com", PasswordHash = "hashedpassword1" },
            new User { Id = 2, Name = "Bob", Email = "bob@example.com", PasswordHash = "hashedpassword2" }
        );

        // Seed Dogs
        modelBuilder.Entity<Dog>().HasData(
            new Dog { Id = 1, Name = "Buddy", Breed = "Labrador", Age = 3, Size = "Large", SpecialNeeds = null, OwnerId = 1 },
            new Dog { Id = 2, Name = "Max", Breed = "Beagle", Age = 5, Size = "Medium", SpecialNeeds = "Allergic to chicken", OwnerId = 2 }
        );

        // Seed Walks
        modelBuilder.Entity<Walk>().HasData(
            new Walk { Id = 1, WalkDate = DateTime.UtcNow.Date, Duration = new TimeSpan(1, 0, 0), Status = "Scheduled", DogId = 1, WalkerId = 2 },
            new Walk { Id = 2, WalkDate = DateTime.UtcNow.Date.AddDays(1), Duration = new TimeSpan(0, 30, 0), Status = "Scheduled", DogId = 2, WalkerId = 1 }
        );
    }
}
