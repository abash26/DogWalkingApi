using DogWalkingApi.Controllers;
using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers;

public class WalkControllerTest
{
    private readonly Mock<IWalkService> _walkServiceMock;
    private readonly WalkController _controller;

    public WalkControllerTest()
    {
        _walkServiceMock = new Mock<IWalkService>();
        _controller = new WalkController(_walkServiceMock.Object);
    }

    #region GetWalks

    [Fact]
    public async Task GetWalks_ShouldReturnOkWithWalks()
    {
        var walks = new List<WalkDto>
        {
            new() { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled },
            new() { Id = 2, StartTime = DateTime.Now.AddHours(1), Duration = TimeSpan.FromMinutes(45), Status = WalkStatus.Scheduled }
        };
        _walkServiceMock.Setup(s => s.GetWalksAsync()).ReturnsAsync(walks);

        var result = await _controller.GetWalks();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(walks);

        _walkServiceMock.Verify(s => s.GetWalksAsync(), Times.Once);
    }

    #endregion

    #region GetWalkById

    [Fact]
    public async Task GetWalkById_ShouldReturnOk_WhenWalkExists()
    {
        var walk = new WalkDto { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled };
        _walkServiceMock.Setup(s => s.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        var result = await _controller.GetWalkById(1);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(walk);

        _walkServiceMock.Verify(s => s.GetWalkByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetWalkById_ShouldReturnNotFound_WhenWalkDoesNotExist()
    {
        _walkServiceMock.Setup(s => s.GetWalkByIdAsync(1)).ReturnsAsync((WalkDto?)null);

        var result = await _controller.GetWalkById(1);

        result.Should().BeOfType<NotFoundResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        _walkServiceMock.Verify(s => s.GetWalkByIdAsync(1), Times.Once);
    }

    #endregion

    #region GetWalksByWalkerId

    [Fact]
    public async Task GetWalksByWalkerId_ShouldReturnOkWithWalks()
    {
        var walks = new List<WalkDto>
        {
            new() { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled },
            new() { Id = 2, StartTime = DateTime.Now.AddHours(1), Duration = TimeSpan.FromMinutes(45), Status = WalkStatus.Scheduled }
        };
        _walkServiceMock.Setup(s => s.GetWalksByWalkerIdAsync(1)).ReturnsAsync(walks);

        var result = await _controller.GetWalksByWalkerId(1);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(walks);

        _walkServiceMock.Verify(s => s.GetWalksByWalkerIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetWalksByWalkerId_ShouldReturnNoContent_WhenNoWalks()
    {
        _walkServiceMock.Setup(s => s.GetWalksByWalkerIdAsync(1)).ReturnsAsync(new List<WalkDto>());

        var result = await _controller.GetWalksByWalkerId(1);

        result.Should().BeOfType<NoContentResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _walkServiceMock.Verify(s => s.GetWalksByWalkerIdAsync(1), Times.Once);
    }

    #endregion

    #region GetWalksByOwnerId

    [Fact]
    public async Task GetWalksByOwnerId_ShouldReturnOkWithWalks()
    {
        var walks = new List<WalkDto>
        {
            new() { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled },
            new() { Id = 2, StartTime = DateTime.Now.AddHours(1), Duration = TimeSpan.FromMinutes(45), Status = WalkStatus.Scheduled }
        };
        _walkServiceMock.Setup(s => s.GetWalksByOwnerIdAsync(1)).ReturnsAsync(walks);

        var result = await _controller.GetWalksByOwnerId(1);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(walks);

        _walkServiceMock.Verify(s => s.GetWalksByOwnerIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetWalksByOwnerId_ShouldReturnNoContent_WhenNoWalks()
    {
        _walkServiceMock.Setup(s => s.GetWalksByOwnerIdAsync(1)).ReturnsAsync(new List<WalkDto>());

        var result = await _controller.GetWalksByOwnerId(1);

        result.Should().BeOfType<NoContentResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _walkServiceMock.Verify(s => s.GetWalksByOwnerIdAsync(1), Times.Once);
    }

    #endregion

    #region ScheduleWalk

    [Fact]
    public async Task ScheduleWalk_ShouldReturnCreatedWalkDto()
    {
        var walkDto = new WalkCreateDto
        {
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromHours(1),
            DogId = 1,
            WalkerId = 2
        };

        var scheduledWalkDto = new WalkDto
        {
            Id = 42,
            StartTime = walkDto.StartTime,
            Duration = walkDto.Duration,
            Status = WalkStatus.Scheduled
        };

        _walkServiceMock
            .Setup(s => s.ScheduleWalkAsync(It.IsAny<Walk>()))
            .ReturnsAsync(scheduledWalkDto);

        var result = await _controller.ScheduleWalk(walkDto);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().BeEquivalentTo(scheduledWalkDto);

        _walkServiceMock.Verify(s => s.ScheduleWalkAsync(It.Is<Walk>(
            w => w.DogId == walkDto.DogId &&
                 w.WalkerId == walkDto.WalkerId &&
                 w.StartTime == walkDto.StartTime &&
                 w.Duration == walkDto.Duration &&
                 w.Status == WalkStatus.Scheduled
        )), Times.Once);
    }

    #endregion

    #region CancelWalk

    [Fact]
    public async Task CancelWalk_ShouldReturnNoContent_WhenCanceled()
    {
        _walkServiceMock.Setup(s => s.CancelWalkByOwnerAsync(1, 123)).Returns(Task.CompletedTask);

        // Mock User claims
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.ControllerContext.HttpContext.User = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "123")
            }, "TestAuth")
        );

        var result = await _controller.CancelWalk(1);

        result.Should().BeOfType<NoContentResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _walkServiceMock.Verify(s => s.CancelWalkByOwnerAsync(1, 123), Times.Once);
    }

    #endregion
}