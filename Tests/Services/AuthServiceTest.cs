using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Repository;
using DogWalkingApi.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Services;
public class AuthServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _service;

    public AuthServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configMock = new Mock<IConfiguration>();

        _configMock.Setup(c => c["Jwt:Key"])
           .Returns("super_secret_test_key_123456789123456");

        _configMock.Setup(c => c["Jwt:Issuer"])
                   .Returns("TestIssuer");

        _configMock.Setup(c => c["Jwt:Audience"])
                   .Returns("TestAudience");

        // Pass the correctly configured mock to AuthService
        _service = new AuthService(_configMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnUser()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "Buddy",
            Email = "user@email.com",
            PasswordHash = "hashedPassword"
        };
        _userRepositoryMock.Setup(repo => repo.GetUserById(1)).ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "Password123!";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = hashedPassword
        };

        var loginDto = new LoginDTO
        {
            Email = "test@test.com",
            Password = password
        };

        _userRepositoryMock.Setup(r => r.GetUserByEmail(loginDto.Email))
                 .ReturnsAsync(user);

        // Act
        var token = await _service.LoginAsync(loginDto);

        // Assert
        token.Should().NotBeNull();
        token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword")
        };

        var loginDto = new LoginDTO
        {
            Email = "test@test.com",
            Password = "WrongPassword"
        };

        _userRepositoryMock.Setup(r => r.GetUserByEmail(loginDto.Email))
                 .ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            Email = "notfound@test.com",
            Password = "Password123"
        };

        _userRepositoryMock.Setup(r => r.GetUserByEmail(loginDto.Email))
                 .ReturnsAsync((User?)null);

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_ReturnsNull_WhenEmailAlreadyExists()
    {
        // Arrange
        var registerDto = new RegisterDTO
        {
            Email = "test@test.com",
            Password = "Password123!"
        };

        _userRepositoryMock.Setup(r => r.GetUserByEmail(registerDto.Email))
                 .ReturnsAsync(new User());

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        result.Should().BeNull();
    }
}
