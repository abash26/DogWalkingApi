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

public class WalkerWalkControllerTest
{
    private readonly Mock<IWalkService> _walkServiceMock;
    private readonly WalkerWalkController _controller;

    public WalkerWalkControllerTest()
    {
        _walkServiceMock = new Mock<IWalkService>();
        _controller = new WalkerWalkController(_walkServiceMock.Object);

        // Setup default User with walkerId = 123
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123")
        }, "TestAuth"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    #region GetAvailableWalks

    [Fact]
    public async Task GetAvailableWalks_ShouldReturnOk_WhenWalksExist()
    {
        var paged = new PagedResult<WalkDto>
        {
            Items =
            [
                new() { Id = 1, Status = WalkStatus.Pending },
                new() { Id = 2, Status = WalkStatus.Pending }
            ],
            TotalCount = 2,
            Page = 1,
            PageSize = 10
        };

        _walkServiceMock
            .Setup(s => s.GetPendingWalksAsync(1, 10))
            .ReturnsAsync(paged);

        var result = await _controller.GetAvailableWalks(1, 10);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(paged);

        _walkServiceMock.Verify(s => s.GetPendingWalksAsync(1, 10), Times.Once);
    }

    [Fact]
    public async Task GetAvailableWalks_ShouldReturnEmpty_WhenNoWalks()
    {
        var paged = new PagedResult<WalkDto>
        {
            Items = [],
            TotalCount = 0,
            Page = 1,
            PageSize = 10
        };

        _walkServiceMock
            .Setup(s => s.GetPendingWalksAsync(1, 10))
            .ReturnsAsync(paged);

        var result = await _controller.GetAvailableWalks(1, 10);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(paged);
    }

    #endregion

    #region GetMyWalks

    [Fact]
    public async Task GetMyWalks_ShouldReturnOk_WhenWalksExist()
    {
        var paged = new PagedResult<WalkDto>
        {
            Items = new List<WalkDto>
        {
            new() { Id = 1 },
            new() { Id = 2 }
        },
            TotalCount = 2,
            Page = 1,
            PageSize = 10
        };

        _walkServiceMock
            .Setup(s => s.GetWalksByWalkerIdAsync(123, 1, 10))
            .ReturnsAsync(paged);

        var result = await _controller.GetMyWalks(1, 10);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(paged);

        _walkServiceMock.Verify(s => s.GetWalksByWalkerIdAsync(123, 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetMyWalks_ShouldReturnEmpty_WhenNoWalks()
    {
        var paged = new PagedResult<WalkDto>
        {
            Items = new List<WalkDto>(),
            TotalCount = 0,
            Page = 1,
            PageSize = 10
        };

        _walkServiceMock
            .Setup(s => s.GetWalksByWalkerIdAsync(123, 1, 10))
            .ReturnsAsync(paged);

        var result = await _controller.GetMyWalks(1, 10);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(paged);
    }

    [Fact]
    public async Task GetMyWalks_ShouldReturnUnauthorized_WhenNoUserId()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await _controller.GetMyWalks();

        result.Should().BeOfType<UnauthorizedResult>();
    }

    #endregion

    #region AcceptWalk

    [Fact]
    public async Task AcceptWalk_ShouldReturnNoContent()
    {
        _walkServiceMock.Setup(s => s.AcceptWalkAsync(1, 123)).Returns(Task.CompletedTask);

        var result = await _controller.AcceptWalk(1);

        result.Should().BeOfType<NoContentResult>();
        _walkServiceMock.Verify(s => s.AcceptWalkAsync(1, 123), Times.Once);
    }

    [Fact]
    public async Task AcceptWalk_ShouldReturnUnauthorized_WhenNoUserId()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await _controller.AcceptWalk(1);

        result.Should().BeOfType<UnauthorizedResult>();
    }

    #endregion

    #region StartWalk

    [Fact]
    public async Task StartWalk_ShouldReturnNoContent()
    {
        _walkServiceMock.Setup(s => s.StartWalkAsync(1, 123)).Returns(Task.CompletedTask);

        var result = await _controller.StartWalk(1);

        result.Should().BeOfType<NoContentResult>();
        _walkServiceMock.Verify(s => s.StartWalkAsync(1, 123), Times.Once);
    }

    [Fact]
    public async Task StartWalk_ShouldReturnUnauthorized_WhenNoUserId()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await _controller.StartWalk(1);

        result.Should().BeOfType<UnauthorizedResult>();
    }

    #endregion

    #region CompleteWalk

    [Fact]
    public async Task CompleteWalk_ShouldReturnNoContent()
    {
        _walkServiceMock.Setup(s => s.CompleteWalkAsync(1, 123)).Returns(Task.CompletedTask);

        var result = await _controller.CompleteWalk(1);

        result.Should().BeOfType<NoContentResult>();
        _walkServiceMock.Verify(s => s.CompleteWalkAsync(1, 123), Times.Once);
    }

    [Fact]
    public async Task CompleteWalk_ShouldReturnUnauthorized_WhenNoUserId()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await _controller.CompleteWalk(1);

        result.Should().BeOfType<UnauthorizedResult>();
    }

    #endregion

    #region CancelWalk

    [Fact]
    public async Task CancelWalk_ShouldReturnNoContent()
    {
        _walkServiceMock.Setup(s => s.CancelWalkByWalkerAsync(1, 123)).Returns(Task.CompletedTask);

        var result = await _controller.CancelWalk(1);

        result.Should().BeOfType<NoContentResult>();
        _walkServiceMock.Verify(s => s.CancelWalkByWalkerAsync(1, 123), Times.Once);
    }

    [Fact]
    public async Task CancelWalk_ShouldReturnUnauthorized_WhenNoUserId()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await _controller.CancelWalk(1);

        result.Should().BeOfType<UnauthorizedResult>();
    }

    #endregion
}