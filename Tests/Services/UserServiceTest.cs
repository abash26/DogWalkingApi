using DogWalkingApi.Models;
using DogWalkingApi.Repository;
using DogWalkingApi.Services;
using FluentAssertions;
using Moq;

namespace Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _service = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUsers()
    {
        var users = new List<User>
        {
            new() { Id = 1, Email = "test@test.com", Name = "User 1", Role = UserRole.Owner },
            new() { Id = 2, Email = "test2@test.com", Name = "User 2", Role = UserRole.Walker }
        };

        _userRepositoryMock
            .Setup(r => r.GetAllUsers())
            .ReturnsAsync(users);

        var result = await _service.GetAllUsersAsync();

        result.Should().HaveCount(2);
        result.First().Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            Name = "Test User",
            Role = UserRole.Owner
        };

        _userRepositoryMock
            .Setup(r => r.GetUserById(1))
            .ReturnsAsync(user);

        var result = await _service.GetUserByIdAsync(1);

        result.Should().NotBeNull();
        result!.Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        _userRepositoryMock
            .Setup(r => r.GetUserById(1))
            .ReturnsAsync((User?)null);

        var result = await _service.GetUserByIdAsync(1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ShouldUpdateRole_WhenUserExists()
    {
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            Name = "Test User",
            Role = UserRole.Owner
        };

        _userRepositoryMock
            .Setup(r => r.GetUserById(1))
            .ReturnsAsync(user);

        var result = await _service.UpdateUserRoleAsync(1, UserRole.Admin);

        result.Should().BeTrue();
        user.Role.Should().Be(UserRole.Admin);

        _userRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        _userRepositoryMock
            .Setup(r => r.GetUserById(1))
            .ReturnsAsync((User?)null);

        var result = await _service.UpdateUserRoleAsync(1, UserRole.Admin);

        result.Should().BeFalse();

        _userRepositoryMock.Verify(
            r => r.UpdateUserAsync(It.IsAny<User>()),
            Times.Never
        );
    }
}