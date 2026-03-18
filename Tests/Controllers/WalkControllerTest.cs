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

    private void SetUser(int userId)
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal(
                    new System.Security.Claims.ClaimsIdentity(
                    [
                        new System.Security.Claims.Claim(
                            System.Security.Claims.ClaimTypes.NameIdentifier,
                            userId.ToString())
                    ], "TestAuth"))
            }
        };
    }

    #region GetWalkById

    [Fact]
    public async Task GetWalkById_ShouldReturnOk_WhenWalkExists()
    {
        var walk = new WalkDto
        {
            Id = 1,
            StartTime = DateTime.UtcNow,
            Duration = TimeSpan.FromMinutes(30),
            Status = WalkStatus.Pending
        };

        _walkServiceMock.Setup(s => s.GetWalkByIdAsync(1))
            .ReturnsAsync(walk);

        var result = await _controller.GetWalkById(1);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(walk);

        _walkServiceMock.Verify(s => s.GetWalkByIdAsync(1), Times.Once);
    }

    #endregion

    #region GetWalkStatus

    [Fact]
    public async Task GetWalkStatus_ShouldReturnStatus_WhenWalkExists()
    {
        var walk = new WalkDto
        {
            Id = 1,
            Status = WalkStatus.Pending
        };

        _walkServiceMock.Setup(s => s.GetWalkByIdAsync(1))
            .ReturnsAsync(walk);

        var result = await _controller.GetWalkStatus(1);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetWalkStatus_ShouldReturnNotFound_WhenWalkDoesNotExist()
    {
        _walkServiceMock.Setup(s => s.GetWalkByIdAsync(1))
            .ReturnsAsync((WalkDto?)null);

        var result = await _controller.GetWalkStatus(1);

        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region GetWalksByOwnerId

    [Fact]
    public async Task GetWalksByOwnerId_ShouldReturnOwnerWalks()
    {
        SetUser(123);

        var walks = new List<WalkDto>
        {
            new() { Id = 1, Status = WalkStatus.Pending },
            new() { Id = 2, Status = WalkStatus.Accepted }
        };

        var pagedResult = new PagedResult<WalkDto>
        {
            Items = walks,
            TotalCount = walks.Count,
            Page = 1,
            PageSize = 10
        };

        _walkServiceMock
            .Setup(s => s.GetWalksByOwnerIdAsync(123, 1, 10))
            .ReturnsAsync(pagedResult);

        var result = await _controller.GetWalksByOwnerId();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(pagedResult);

        _walkServiceMock.Verify(s => s.GetWalksByOwnerIdAsync(123, 1, 10), Times.Once);
    }

    #endregion

    #region ScheduleWalk

    [Fact]
    public async Task ScheduleWalk_ShouldReturnCreatedWalk()
    {
        SetUser(123);

        var walkDto = new WalkCreateDto
        {
            DogId = 1,
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(30)
        };

        var scheduledWalk = new WalkDto
        {
            Id = 10,
            StartTime = walkDto.StartTime,
            Duration = walkDto.Duration,
            Status = WalkStatus.Pending
        };

        _walkServiceMock
            .Setup(s => s.ScheduleWalkAsync(It.IsAny<Walk>()))
            .ReturnsAsync(scheduledWalk);

        var result = await _controller.ScheduleWalk(walkDto);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().BeEquivalentTo(scheduledWalk);

        _walkServiceMock.Verify(s => s.ScheduleWalkAsync(It.Is<Walk>(w =>
            w.DogId == walkDto.DogId &&
            w.StartTime == walkDto.StartTime &&
            w.Duration == walkDto.Duration &&
            w.OwnerId == 123 &&
            w.Status == WalkStatus.Pending
        )), Times.Once);
    }

    #endregion

    #region CancelWalk

    [Fact]
    public async Task CancelWalk_ShouldReturnNoContent_WhenCanceled()
    {
        SetUser(123);

        _walkServiceMock.Setup(s => s.CancelWalkByOwnerAsync(1, 123))
            .Returns(Task.CompletedTask);

        var result = await _controller.CancelWalk(1);

        result.Should().BeOfType<NoContentResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _walkServiceMock.Verify(s => s.CancelWalkByOwnerAsync(1, 123), Times.Once);
    }

    #endregion
}