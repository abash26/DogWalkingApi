using DogWalkingApi.Data;
using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Repository;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;

namespace Tests.Repositories;

public class DogRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;
    private readonly DogRepository _repository;

    public DogRepositoryTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options, seedData: false);
        _context.Database.EnsureCreated();

        _repository = new DogRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    [Fact]
    public async Task GetDogs_ShouldReturnAllDogs()
    {
        // Arrange
        await TestHelpers.AddTestDog(_context, "Buddy", 1);
        await TestHelpers.AddTestDog(_context, "Max", 2);

        // Act
        var dogs = await _repository.GetDogs();

        // Assert
        dogs.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetDogs_WhenNoDogsExist_ShouldReturnEmptyList()
    {
        // Act
        var dogs = await _repository.GetDogs();

        // Assert
        dogs.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1, "Buddy")]
    [InlineData(2, "Max")]
    public async Task GetDogById_WhenCalled_ShouldReturnCorrectDog(int id, string expectedName)
    {
        // Arrange
        await TestHelpers.AddTestDog(_context, "Buddy", 1);
        await TestHelpers.AddTestDog(_context, "Max", 2);

        // Act
        var dog = await _repository.GetDogById(id);

        // Assert
        dog?.Name.Should().Be(expectedName);
    }

    [Fact]
    public async Task GetDogById_WhenDogDoesNotExist_ShouldReturnNull()
    {
        // Act
        var dog = await _repository.GetDogById(999); // Non-existent ID

        // Assert
        dog.Should().BeNull();
    }

    [Fact]
    public async Task AddDog_ShouldReturnNewDog()
    {
        // Arrange
        var newDog = new CreateDogDTO
        {
            Name = "Jerry",
            Breed = "Labrador",
            Age = 3,
            Size = "Medium",
            SpecialNeeds = "None",
            OwnerId = 1
        };

        // Act
        var dog = await _repository.AddDog(newDog);

        // Assert
        dog.Should().NotBeNull();
        dog.Name.Should().Be("Jerry");
        dog.Breed.Should().Be("Labrador");
        dog.Age.Should().Be(3);
        dog.Size.Should().Be("Medium");
        dog.SpecialNeeds.Should().Be("None");
        dog.OwnerId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateDog_ShouldReturnDog()
    {
        // Arrange
        await TestHelpers.AddTestDog(_context, "Buddy", 1);

        var dogToUpdate = new Dog
        {
            Id = 1,
            Name = "Zsofi",
            Size = "Medium",
            OwnerId = 1
        };

        // Act
        var dog = await _repository.UpdateDog(dogToUpdate);

        // Assert
        dog?.Name.Should().Be("Zsofi");
    }

    [Fact]
    public async Task UpdateDog_WhenDogDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentDog = new Dog
        {
            Id = 999, // Non-existent ID
            Name = "NonExistent",
            OwnerId = 1
        };

        // Act
        var result = await _repository.UpdateDog(nonExistentDog);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteDog_ShouldDeleteIt()
    {
        // Arrange
        var dog = await TestHelpers.AddTestDog(_context, "Zsofi", 1);

        // Act
        await _repository.DeleteDog(dog);

        // Assert
        var deletedDog = await _context.Dogs.FindAsync(dog.Id);
        deletedDog.Should().BeNull();
    }
}
