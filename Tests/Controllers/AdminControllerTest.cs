using DogWalkingApi.Controllers;
using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers;
public class AdminControllerTests
{
    private readonly Mock<IDogService> _dogServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IWalkService> _walkServiceMock;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _dogServiceMock = new Mock<IDogService>();
        _userServiceMock = new Mock<IUserService>();
        _walkServiceMock = new Mock<IWalkService>();
        _controller = new AdminController(
            _userServiceMock.Object,
            _walkServiceMock.Object,
            _dogServiceMock.Object
        );
    }
    [Fact]
    public async Task GetUsers_ShouldReturnOkWithUsers_WhenUsersExist()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Email = "test@test.com", Role = UserRole.Owner }
        };
        _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task UpdateUserRole_ReturnsNoContent_WhenSuccessful()
    {
        _userServiceMock
            .Setup(s => s.UpdateUserRoleAsync(1, UserRole.Admin))
            .ReturnsAsync(true);

        var result = await _controller.UpdateUserRole(1, UserRole.Admin);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateUserRole_ReturnsNotFound_WhenUserMissing()
    {
        _userServiceMock
            .Setup(s => s.UpdateUserRoleAsync(1, UserRole.Admin))
            .ReturnsAsync(false);

        var result = await _controller.UpdateUserRole(1, UserRole.Admin);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAllWalks_ReturnsOkResult()
    {
        var walks = new List<WalkDto>
        {
            new() { Id = 1, Status = WalkStatus.Pending },
            new() { Id = 2, Status = WalkStatus.Accepted }
        };

        _walkServiceMock.Setup(s => s.GetWalksAsync())
                .ReturnsAsync(walks);

        var result = await _controller.GetAllWalks();

        result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(walks);

        _walkServiceMock.Verify(s => s.GetWalksAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelWalk_ReturnsNoContent()
    {
        _walkServiceMock
            .Setup(s => s.CancelWalk(1))
            .Returns(Task.CompletedTask);

        var result = await _controller.CancelWalk(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetAllDogs_ShouldReturnOkWithDogs_WhenDogsExist()
    {
        // Arrange
        var dogs = new List<Dog>
        {
            new() { Id = 1, Name = "Rex", Age = 3, Size = "Medium", OwnerId = 1 },
            new() { Id = 2, Name = "Bella", Age = 5, Size = "Small", OwnerId = 2 }
        };
        _dogServiceMock.Setup(s => s.GetAllDogsAsync()).ReturnsAsync(dogs);

        // Act
        var result = await _controller.GetAllDogs();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dogs);
    }

}
