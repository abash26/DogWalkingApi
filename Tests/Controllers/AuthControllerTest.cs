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
public class AuthControllerTest
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTest()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    #region Helpers

    private ClaimsPrincipal CreateUserPrincipal(int userId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    #endregion

    #region Login

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
    {
        // Arrange
        var loginDto = new LoginDTO { Email = "testuser@test.com", Password = "wrongpassword" };
        _authServiceMock.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync((string?)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorized.Value.Should().Be("Invalid credentials.");
        unauthorized.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);

        _authServiceMock.Verify(s => s.LoginAsync(loginDto), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenValidCredentials()
    {
        // Arrange
        var loginDto = new LoginDTO { Email = "testuser@test.com", Password = "password" };
        _authServiceMock.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync("valid_token");

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { Token = "valid_token" });
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        _authServiceMock.Verify(s => s.LoginAsync(loginDto), Times.Once);
    }

    #endregion

    #region Register

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var registerDto = new RegisterDTO { Email = "testuser@test.com", Password = "password", Name = "Test User" };
        _authServiceMock.Setup(s => s.RegisterAsync(registerDto)).ReturnsAsync((string?)null);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var conflict = result.Should().BeOfType<ConflictObjectResult>().Subject;
        conflict.Value.Should().Be("User already exists.");
        conflict.StatusCode.Should().Be(StatusCodes.Status409Conflict);

        _authServiceMock.Verify(s => s.RegisterAsync(registerDto), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldReturnToken_WhenRegistrationSucceeds()
    {
        // Arrange
        var registerDto = new RegisterDTO { Email = "testuser@test.com", Password = "password", Name = "Test User" };
        _authServiceMock.Setup(s => s.RegisterAsync(registerDto)).ReturnsAsync("new_token");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { Token = "new_token" });
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        _authServiceMock.Verify(s => s.RegisterAsync(registerDto), Times.Once);
    }

    #endregion

    #region GetMe

    [Fact]
    public async Task GetMe_ShouldReturnUser_WhenAuthenticated()
    {
        // Arrange
        var user = new User { Id = 1, Name = "Test User", Email = "testuser@test.com" };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserPrincipal(user.Id) }
        };

        _authServiceMock.Setup(s => s.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetMe();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { user.Id, user.Email });
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        _authServiceMock.Verify(s => s.GetUserByIdAsync(user.Id), Times.Once);
    }

    [Fact]
    public async Task GetMe_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() // no user
        };

        // Act
        var result = await _controller.GetMe();

        // Assert
        var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorized.Value.Should().BeEquivalentTo(new { Message = "Invalid token" });
        unauthorized.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task GetMe_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateUserPrincipal(42) }
        };

        _authServiceMock.Setup(s => s.GetUserByIdAsync(42)).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.GetMe();

        // Assert
        var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.Value.Should().BeEquivalentTo(new { Message = "User not found" });
        notFound.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    #region Logout

    [Fact]
    public void Logout_ShouldReturnOk()
    {
        // Act
        var result = _controller.Logout();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { Message = "Logout successful." });
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    #endregion
}
