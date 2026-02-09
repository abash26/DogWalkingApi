using DogWalkingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DogWalkingApi.Data;

public class ApplicationDbContext : DbContext
{
    private readonly bool _seedData;
    public ApplicationDbContext(
       DbContextOptions<ApplicationDbContext> options,
       bool seedData = true)
       : base(options)
    {
        _seedData = seedData;
    }

    public DbSet<Dog> Dogs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Walk> Walks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (_seedData)
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
                new Walk
                {
                    Id = 1,
                    StartTime = new DateTime(2026, 2, 5, 10, 0, 0),
                    Duration = TimeSpan.FromHours(1), // okay for EF Core 6+, else use ticks
                    Status = WalkStatus.Scheduled,
                    DogId = 1,
                    WalkerId = 2
                },
                new Walk
                {
                    Id = 2,
                    StartTime = new DateTime(2026, 2, 6, 14, 0, 0),
                    Duration = TimeSpan.FromMinutes(30),
                    Status = WalkStatus.Scheduled,
                    DogId = 2,
                    WalkerId = 1
                }
            );
        }
    }
}
