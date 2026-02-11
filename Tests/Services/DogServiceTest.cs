using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Repository;
using DogWalkingApi.Services;
using FluentAssertions;
using Moq;

namespace Tests.Services;
public class DogServiceTest
{
    private readonly Mock<IDogRepository> _dogRepositoryMock;
    private readonly DogService _service;

    public DogServiceTest()
    {
        _dogRepositoryMock = new Mock<IDogRepository>();
        _service = new DogService(_dogRepositoryMock.Object);
    }

    [Fact]
    public async Task GetDogsAsync_ReturnAllDogs()
    {
        // Arrange
        var dogs = new List<Dog>
        {
            new() { Id = 1, Name = "Buddy", Breed = "Golden Retriever", Age = 3, Size = "Large", OwnerId = 1 },
            new() { Id = 2, Name = "Max", Breed = "Labrador Retriever", Age = 5, Size = "Large", OwnerId = 2 }
        };
        _dogRepositoryMock.Setup(repo => repo.GetDogs()).ReturnsAsync(dogs);

        // Act
        var result = await _service.GetDogsAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Buddy", result[0].Name);
        Assert.Equal("Max", result[1].Name);
    }

    [Fact]
    public async Task GetDogByIdAsync_ReturnDog()
    {
        // Arrange
        var dog = new Dog { Id = 1, Name = "Buddy", Breed = "Golden Retriever", Age = 3, Size = "Large", OwnerId = 1 };
        _dogRepositoryMock.Setup(repo => repo.GetDogById(1)).ReturnsAsync(dog);

        // Act
        var result = await _service.GetDogByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(dog);
    }

    [Fact]
    public async Task AddDogAsync_AddAndReturnDog()
    {
        // Arrange
        var createDogDto = new CreateDogDTO { Name = "Buddy", Breed = "Golden Retriever", Age = 3, Size = "Large", OwnerId = 1 };
        var addedDog = new Dog { Id = 1, Name = "Buddy", Breed = "Golden Retriever", Age = 3, Size = "Large", OwnerId = 1 };
        _dogRepositoryMock.Setup(repo => repo.AddDog(createDogDto)).ReturnsAsync(addedDog);

        // Act
        var result = await _service.AddDogAsync(createDogDto);

        // Assert
        result.Should().BeEquivalentTo(addedDog);
    }

    [Fact]
    public async Task UpdateDogAsync_UpdatesOnlyProvidedFields()
    {
        var dog = new Dog { Id = 1, Name = "Buddy", Breed = "Golden Retriever", Age = 3, Size = "Large", OwnerId = 1 };
        var updateDto = new UpdateDogDTO { Name = "Buddy Updated", Age = 4 };
        _dogRepositoryMock.Setup(repo => repo.GetDogById(1)).ReturnsAsync(dog);
        _dogRepositoryMock.Setup(r => r.UpdateDog(It.IsAny<Dog>())).ReturnsAsync((Dog d) => d);

        // Act
        var result = await _service.UpdateDogAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Buddy Updated");
        result.Age.Should().Be(4);
        result.Breed.Should().Be("Golden Retriever");
    }

    [Fact]
    public async Task DeleteDogAsync_ReturnsFalseIfDogNotFound()
    {
        // Arrange
        _dogRepositoryMock.Setup(repo => repo.GetDogById(1)).ReturnsAsync((Dog?)null);

        // Act
        var result = await _service.DeleteDogAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteDogAsync_ReturnsTrue()
    {
        // Arrange
        var dog = new Dog { Id = 1, Name = "Buddy" };

        _dogRepositoryMock.Setup(r => r.GetDogById(1))
                 .ReturnsAsync(dog);

        _dogRepositoryMock.Setup(r => r.DeleteDog(dog))
                 .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteDogAsync(1);

        // Assert
        result.Should().BeTrue();
    }
}