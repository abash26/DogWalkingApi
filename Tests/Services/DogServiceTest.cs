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
    public async Task GetAllDogsAsync_ShouldReturnDogs()
    {
        // Arrange
        var ownerId = 1;
        var dogs = new List<Dog>
        {
            new() { Id = 1, Name = "Buddy", Breed = "Golden Retriever", Age = 3, Size = "Large", OwnerId = ownerId },
            new() { Id = 2, Name = "Max", Breed = "Labrador", Age = 5, Size = "Large", OwnerId = ownerId }
        };
        _dogRepositoryMock.Setup(r => r.GetDogs()).ReturnsAsync(dogs);

        // Act
        var result = await _service.GetAllDogsAsync();

        // Assert
        result.Should().BeEquivalentTo(dogs);
    }

    [Fact]
    public async Task GetDogsAsync_ShouldReturnDogsForOwner()
    {
        // Arrange
        var ownerId = 1;
        var page = 1;
        var pageSize = 10;

        var dogs = new List<Dog>
    {
        new() { Id = 1, Name = "Buddy", Breed = "Golden Retriever", Age = 3, Size = "Large", OwnerId = ownerId },
        new() { Id = 2, Name = "Max", Breed = "Labrador", Age = 5, Size = "Large", OwnerId = ownerId }
    };

        _dogRepositoryMock
            .Setup(r => r.GetDogsByOwnerId(ownerId, page, pageSize))
            .ReturnsAsync((dogs, dogs.Count));

        // Act
        var result = await _service.GetDogsAsync(ownerId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(dogs);
        result.TotalCount.Should().Be(dogs.Count);
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
    }

    [Fact]
    public async Task GetDogByIdAsync_ShouldReturnDog()
    {
        // Arrange
        var dog = new Dog { Id = 1, Name = "Buddy", Breed = "Golden Retriever", Age = 3, Size = "Large", OwnerId = 1 };
        _dogRepositoryMock.Setup(r => r.GetDogById(1)).ReturnsAsync(dog);

        // Act
        var result = await _service.GetDogByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(dog);
    }

    [Fact]
    public async Task AddDogAsync_ShouldAddAndReturnDog()
    {
        // Arrange
        var ownerId = 1;
        var createDto = new CreateDogDTO
        {
            Name = "Buddy",
            Breed = "Golden Retriever",
            Age = 3,
            Size = "Large",
            SpecialNeeds = "None"
        };

        var addedDog = new Dog
        {
            Id = 1,
            Name = "Buddy",
            Breed = "Golden Retriever",
            Age = 3,
            Size = "Large",
            SpecialNeeds = "None",
            OwnerId = ownerId
        };

        _dogRepositoryMock
            .Setup(r => r.AddDog(It.Is<Dog>(d =>
                d.Name == createDto.Name &&
                d.Breed == createDto.Breed &&
                d.Age == createDto.Age &&
                d.Size == createDto.Size &&
                d.SpecialNeeds == createDto.SpecialNeeds &&
                d.OwnerId == ownerId)))
            .ReturnsAsync(addedDog);

        // Act
        var result = await _service.AddDogAsync(createDto, ownerId);

        // Assert
        result.Should().BeEquivalentTo(addedDog);
    }

    [Fact]
    public async Task UpdateDogAsync_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var ownerId = 1;
        var dog = new Dog { Id = 1, Name = "Buddy", Breed = "Golden Retriever", Age = 3, Size = "Large", OwnerId = ownerId };
        var updateDto = new UpdateDogDTO { Name = "Buddy Updated", Age = 4 };

        _dogRepositoryMock.Setup(r => r.GetDogById(1)).ReturnsAsync(dog);
        _dogRepositoryMock.Setup(r => r.UpdateDog(It.IsAny<Dog>())).ReturnsAsync((Dog d) => d);

        // Act
        var result = await _service.UpdateDogAsync(1, updateDto, ownerId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Buddy Updated");
        result.Age.Should().Be(4);
        result.Breed.Should().Be("Golden Retriever");
        result.Size.Should().Be("Large");
    }

    [Fact]
    public async Task UpdateDogAsync_ShouldReturnNull_IfDogNotFound()
    {
        // Arrange
        var ownerId = 1;
        _dogRepositoryMock.Setup(r => r.GetDogById(1)).ReturnsAsync((Dog?)null);
        var updateDto = new UpdateDogDTO { Name = "Updated" };

        // Act
        var result = await _service.UpdateDogAsync(1, updateDto, ownerId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateDogAsync_ShouldReturnNull_IfOwnerMismatch()
    {
        // Arrange
        var dog = new Dog { Id = 1, Name = "Buddy", OwnerId = 2 }; // different owner
        var ownerId = 1;
        _dogRepositoryMock.Setup(r => r.GetDogById(1)).ReturnsAsync(dog);
        var updateDto = new UpdateDogDTO { Name = "Updated" };

        // Act
        var result = await _service.UpdateDogAsync(1, updateDto, ownerId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteDogAsync_ShouldReturnTrue()
    {
        // Arrange
        var ownerId = 1;
        var dog = new Dog { Id = 1, Name = "Buddy", OwnerId = ownerId };
        _dogRepositoryMock.Setup(r => r.GetDogById(1)).ReturnsAsync(dog);
        _dogRepositoryMock.Setup(r => r.DeleteDog(dog)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteDogAsync(1, ownerId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteDogAsync_ShouldReturnFalse_IfDogNotFound()
    {
        // Arrange
        var ownerId = 1;
        _dogRepositoryMock.Setup(r => r.GetDogById(1)).ReturnsAsync((Dog?)null);

        // Act
        var result = await _service.DeleteDogAsync(1, ownerId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteDogAsync_ShouldReturnFalse_IfOwnerMismatch()
    {
        // Arrange
        var ownerId = 1;
        var dog = new Dog { Id = 1, Name = "Buddy", OwnerId = 2 }; // different owner
        _dogRepositoryMock.Setup(r => r.GetDogById(1)).ReturnsAsync(dog);

        // Act
        var result = await _service.DeleteDogAsync(1, ownerId);

        // Assert
        result.Should().BeFalse();
    }
}