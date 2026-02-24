using DogWalkingApi.Controllers;
using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Tests.Controllers;
public class DogControllerTest
{
    private readonly Mock<IDogService> _dogServiceMock;
    private readonly DogController _controller;

    public DogControllerTest()
    {
        _dogServiceMock = new Mock<IDogService>();
        _controller = new DogController(_dogServiceMock.Object);
    }

    private void SetUser(int userId, string role = "Owner")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    #region GetDogs

    [Fact]
    public async Task GetDogs_ShouldReturnOkWithDogs_WhenDogsExist()
    {
        // Arrange
        var userId = 1;
        SetUser(userId);
        var dogs = new List<Dog>
        {
            new() { Id = 1, Name = "Rex", Age = 3, Size = "Medium", OwnerId = 1 },
            new() { Id = 2, Name = "Bella", Age = 5, Size = "Small", OwnerId = 2 }
        };
        _dogServiceMock.Setup(s => s.GetDogsAsync(userId)).ReturnsAsync(dogs);

        // Act
        var result = await _controller.GetDogs();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dogs);
    }

    [Fact]
    public async Task GetDogs_ShouldReturnNoContent_WhenNoDogsExist()
    {
        // Arrange
        var userId = 1;
        SetUser(userId);
        _dogServiceMock.Setup(s => s.GetDogsAsync(userId)).ReturnsAsync(new List<Dog>());

        // Act
        var result = await _controller.GetDogs();

        // Assert
        result.Should().BeOfType<NoContentResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    #endregion

    #region GetDogById

    [Fact]
    public async Task GetDogById_ShouldReturnDog_WhenDogExists()
    {
        // Arrange
        var dog = new Dog { Id = 1, Name = "Rex", Age = 3, Size = "Medium", OwnerId = 1 };
        _dogServiceMock.Setup(s => s.GetDogByIdAsync(1)).ReturnsAsync(dog);

        // Act
        var result = await _controller.GetDogById(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dog);
    }

    [Fact]
    public async Task GetDogById_ShouldReturnNotFound_WhenDogDoesNotExist()
    {
        // Arrange
        _dogServiceMock.Setup(s => s.GetDogByIdAsync(999)).ReturnsAsync((Dog?)null);

        // Act
        var result = await _controller.GetDogById(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    #region CreateDog

    [Fact]
    public async Task CreateDog_ShouldReturnCreatedDog_WhenDtoIsValid()
    {
        // Arrange
        var userId = 1;
        SetUser(userId);
        var createDto = new CreateDogDTO { Name = "Rex", Age = 3, Size = "Medium" };
        var createdDog = new Dog { Id = 1, Name = "Rex", Age = 3, Size = "Medium", OwnerId = userId };
        _dogServiceMock.Setup(s => s.AddDogAsync(createDto, userId)).ReturnsAsync(createdDog);

        // Act
        var result = await _controller.CreateDog(createDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().BeEquivalentTo(createdDog);

        _dogServiceMock.Verify(s => s.AddDogAsync(createDto, userId), Times.Once);
    }

    [Fact]
    public async Task CreateDog_ShouldReturnBadRequest_WhenDtoIsNull()
    {
        // Act
        var result = await _controller.CreateDog(null!);

        // Assert
        result.Should().BeOfType<BadRequestResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        _dogServiceMock.Verify(s => s.AddDogAsync(It.IsAny<CreateDogDTO>(), It.IsAny<int>()), Times.Never);
    }

    #endregion

    #region UpdateDog

    [Fact]
    public async Task UpdateDog_ShouldReturnUpdatedDog_WhenDogExists()
    {
        // Arrange
        var userId = 1;
        SetUser(userId);
        var updateDto = new UpdateDogDTO { Name = "Rex", Age = 4, Size = "Medium" };
        var updatedDog = new Dog { Id = 1, Name = "Rex", Age = 4, Size = "Medium", OwnerId = 1 };
        _dogServiceMock.Setup(s => s.UpdateDogAsync(1, updateDto, userId)).ReturnsAsync(updatedDog);

        // Act
        var result = await _controller.UpdateDog(1, updateDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(updatedDog);

        _dogServiceMock.Verify(s => s.UpdateDogAsync(1, updateDto, userId), Times.Once);
    }

    [Fact]
    public async Task UpdateDog_ShouldReturnBadRequest_WhenDtoIsNull()
    {
        // Act
        var result = await _controller.UpdateDog(1, null!);

        // Assert
        result.Should().BeOfType<BadRequestResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        _dogServiceMock.Verify(s => s.UpdateDogAsync(It.IsAny<int>(), It.IsAny<UpdateDogDTO>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateDog_ShouldReturnNotFound_WhenDogDoesNotExist()
    {
        // Arrange
        var userId = 1;
        SetUser(userId);
        var updateDto = new UpdateDogDTO { Name = "Rex", Age = 4, Size = "Medium" };
        _dogServiceMock.Setup(s => s.UpdateDogAsync(1, updateDto, userId)).ReturnsAsync((Dog?)null);

        // Act
        var result = await _controller.UpdateDog(1, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        _dogServiceMock.Verify(s => s.UpdateDogAsync(1, updateDto, userId), Times.Once);
    }

    #endregion

    #region DeleteDog

    [Fact]
    public async Task DeleteDog_ShouldReturnNoContent_WhenDeleted()
    {
        // Arrange
        var userId = 1;
        SetUser(userId);
        _dogServiceMock.Setup(s => s.DeleteDogAsync(1, userId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteDog(1);

        // Assert
        result.Should().BeOfType<NoContentResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _dogServiceMock.Verify(s => s.DeleteDogAsync(1, userId), Times.Once);
    }

    [Fact]
    public async Task DeleteDog_ShouldReturnNotFound_WhenDogDoesNotExist()
    {
        // Arrange
        var userId = 1;
        SetUser(userId);
        _dogServiceMock.Setup(s => s.DeleteDogAsync(1, userId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteDog(1);

        // Assert
        result.Should().BeOfType<NotFoundResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion
}
