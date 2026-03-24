using DogWalkingApi.Models;
using DogWalkingApi.Repository;
using DogWalkingApi.Services;
using FluentAssertions;
using Moq;

namespace Tests.Services;

public class WalkServiceTest
{
    private readonly Mock<IWalkRepository> _walkRepositoryMock;
    private readonly WalkService _service;

    public WalkServiceTest()
    {
        _walkRepositoryMock = new Mock<IWalkRepository>();
        _service = new WalkService(_walkRepositoryMock.Object);
    }

    [Fact]
    public async Task GetWalksAsync_ShouldReturnWalks()
    {
        var walks = new List<Walk>
        {
            new() {
                Id = 1,
                StartTime = DateTime.Now,
                Duration = TimeSpan.FromMinutes(30),
                Status = WalkStatus.Pending
            },
            new() {
                Id = 2,
                StartTime = DateTime.Now.AddHours(1),
                Duration = TimeSpan.FromMinutes(45),
                Status = WalkStatus.Pending
            }
        };
        _walkRepositoryMock.Setup(repo => repo.GetWalksAsync()).ReturnsAsync(walks);

        var result = await _service.GetWalksAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetWalkByIdAsync_ShouldReturnWalk()
    {
        var walk = new Walk
        {
            Id = 1,
            StartTime = DateTime.Now,
            Duration = TimeSpan.FromMinutes(30),
            Status = WalkStatus.Pending
        };
        _walkRepositoryMock.Setup(repo => repo.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        var result = await _service.GetWalkByIdAsync(1);

        result!.Status.Should().Be(WalkStatus.Pending);
    }

    [Fact]
    public async Task GetWalkByIdAsync_ShouldReturnNull_WhenWalkNotFound()
    {
        _walkRepositoryMock.Setup(repo => repo.GetWalkByIdAsync(1)).ReturnsAsync((Walk?)null);

        var result = await _service.GetWalkByIdAsync(1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetWalksByWalkerIdAsync_ShouldReturnWalks()
    {
        int page = 1;
        int pageSize = 10;
        int totalCount = 2;

        var walks = new List<Walk>
        {
            new() {
                Id = 1,
                StartTime = DateTime.Now,
                Duration = TimeSpan.FromMinutes(30),
                Status = WalkStatus.Pending,
                WalkerId = 1
            },
            new() {
                Id = 2,
                StartTime = DateTime.Now.AddHours(1),
                Duration = TimeSpan.FromMinutes(45),
                Status = WalkStatus.Pending,
                WalkerId = 1
            }
        };

        _walkRepositoryMock
            .Setup(repo => repo.GetWalksByWalkerIdAsync(1, page, pageSize))
            .ReturnsAsync((walks, totalCount));

        var result = await _service.GetWalksByWalkerIdAsync(1, page, pageSize);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetWalksByOwnerIdAsync_ShouldReturnPagedWalks()
    {
        // Arrange
        int ownerId = 1;
        int page = 1;
        int pageSize = 10;

        var walksFromRepo = new List<Walk>
    {
        new() {
            Id = 1,
            StartTime = DateTime.Now,
            Duration = TimeSpan.FromMinutes(30),
            Status = WalkStatus.Pending,
            OwnerId = ownerId
        },
        new() {
            Id = 2,
            StartTime = DateTime.Now.AddHours(1),
            Duration = TimeSpan.FromMinutes(45),
            Status = WalkStatus.Accepted,
            OwnerId = ownerId
        }
    };

        int totalCount = 2;

        _walkRepositoryMock
            .Setup(repo => repo.GetWalksByOwnerIdAsync(ownerId, page, pageSize))
            .ReturnsAsync((walksFromRepo, totalCount));

        // Act
        var result = await _service.GetWalksByOwnerIdAsync(ownerId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(totalCount);
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);

        result.Items[0].Id.Should().Be(1);
        result.Items[1].Status.Should().Be(WalkStatus.Accepted);

        _walkRepositoryMock.Verify(
            repo => repo.GetWalksByOwnerIdAsync(ownerId, page, pageSize),
            Times.Once
        );
    }

    [Fact]
    public async Task ScheduleWalkAsync_ShouldScheduleWalk()
    {
        var walk = new Walk { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30) };
        _walkRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Walk>())).Returns(Task.CompletedTask);
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        var result = await _service.ScheduleWalkAsync(new Walk { Id = 1, StartTime = walk.StartTime, Duration = walk.Duration });

        result.Status.Should().Be(WalkStatus.Pending);
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task AcceptWalkAsync_ShouldAcceptWalk_WhenPending()
    {
        var walk = new Walk { Id = 1, Status = WalkStatus.Pending };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);
        _walkRepositoryMock.Setup(r => r.AcceptWalkAsync(1, 123)).ReturnsAsync(true);

        await _service.AcceptWalkAsync(1, 123);

        _walkRepositoryMock.Verify(r => r.AcceptWalkAsync(1, 123), Times.Once);
    }

    [Fact]
    public async Task AcceptWalkAsync_ShouldThrow_WhenAlreadyAccepted()
    {
        var walk = new Walk { Id = 1, Status = WalkStatus.Pending };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);
        _walkRepositoryMock.Setup(r => r.AcceptWalkAsync(1, 123)).ReturnsAsync(false);

        Func<Task> act = async () => await _service.AcceptWalkAsync(1, 123);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Walk already accepted.");
    }

    [Fact]
    public async Task CompleteWalkAsync_ShouldCompleteWalk_WhenWalkerAndInProgress()
    {
        var walk = new Walk { Id = 1, WalkerId = 123, Status = WalkStatus.InProgress };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);
        _walkRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Walk>())).Returns(Task.CompletedTask);

        await _service.CompleteWalkAsync(1, 123);

        walk.Status.Should().Be(WalkStatus.Completed);
        _walkRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Walk>(w => w.Status == WalkStatus.Completed)), Times.Once);
    }

    [Fact]
    public async Task CompleteWalkAsync_ShouldThrow_WhenWrongWalker()
    {
        var walk = new Walk { Id = 1, WalkerId = 999, Status = WalkStatus.InProgress };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        Func<Task> act = async () => await _service.CompleteWalkAsync(1, 123);

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("You are not assigned to this walk");
    }

    [Fact]
    public async Task CompleteWalkAsync_ShouldThrow_WhenNotInProgress()
    {
        var walk = new Walk { Id = 1, WalkerId = 123, Status = WalkStatus.Accepted };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        Func<Task> act = async () => await _service.CompleteWalkAsync(1, 123);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Cannot change walk status from Accepted to Completed");
    }

    [Fact]
    public async Task CancelWalkByWalkerAsync_ShouldCancelWalk()
    {
        var walk = new Walk { Id = 1, WalkerId = 123, Status = WalkStatus.Accepted };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);
        _walkRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Walk>())).Returns(Task.CompletedTask);

        await _service.CancelWalkByWalkerAsync(1, 123);

        walk.Status.Should().Be(WalkStatus.Cancelled);
    }

    [Fact]
    public async Task CancelWalkByOwnerAsync_ShouldCancelWalk()
    {
        var walk = new Walk { Id = 1, OwnerId = 456, Status = WalkStatus.Accepted };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);
        _walkRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Walk>())).Returns(Task.CompletedTask);

        await _service.CancelWalkByOwnerAsync(1, 456);

        walk.Status.Should().Be(WalkStatus.Cancelled);
    }

    [Fact]
    public async Task CancelWalk_ShouldCancelWalk()
    {
        var walk = new Walk { Id = 1, OwnerId = 456, Status = WalkStatus.Accepted };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);
        _walkRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Walk>())).Returns(Task.CompletedTask);

        await _service.CancelWalk(1);

        walk.Status.Should().Be(WalkStatus.Cancelled);
    }
}