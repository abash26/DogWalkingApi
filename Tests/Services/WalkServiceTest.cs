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
            new() { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled },
            new() { Id = 2, StartTime = DateTime.Now.AddHours(1), Duration = TimeSpan.FromMinutes(45), Status = WalkStatus.Scheduled }
        };
        _walkRepositoryMock.Setup(repo => repo.GetWalksAsync()).ReturnsAsync(walks);

        var result = await _service.GetWalksAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetWalkByIdAsync_ShouldReturnWalk()
    {
        var walk = new Walk { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled };
        _walkRepositoryMock.Setup(repo => repo.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        var result = await _service.GetWalkByIdAsync(1);

        result!.Status.Should().Be(WalkStatus.Scheduled);
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
        var walks = new List<Walk>
        {
            new() { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled, WalkerId = 1 },
            new() { Id = 2, StartTime = DateTime.Now.AddHours(1), Duration = TimeSpan.FromMinutes(45), Status = WalkStatus.Scheduled, WalkerId = 1 }
        };
        _walkRepositoryMock.Setup(repo => repo.GetWalksByWalkerIdAsync(1)).ReturnsAsync(walks);

        var result = await _service.GetWalksByWalkerIdAsync(1);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetWalksByWalkerIdAsync_ShouldReturnEmptyList_WhenNoWalksFound()
    {
        _walkRepositoryMock.Setup(repo => repo.GetWalksByWalkerIdAsync(1)).ReturnsAsync(new List<Walk>());

        var result = await _service.GetWalksByWalkerIdAsync(1);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetWalksByOwnerIdAsync_ShouldReturnWalks()
    {
        var walks = new List<Walk>
        {
            new() { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled, OwnerId = 1 },
            new() { Id = 2, StartTime = DateTime.Now.AddHours(1), Duration = TimeSpan.FromMinutes(45), Status = WalkStatus.Scheduled, OwnerId = 1 }
        };
        _walkRepositoryMock.Setup(repo => repo.GetWalksByOwnerIdAsync(1)).ReturnsAsync(walks);

        var result = await _service.GetWalksByOwnerIdAsync(1);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task ScheduleWalkAsync_ShouldScheduleWalk()
    {
        var walk = new Walk { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30) };
        _walkRepositoryMock.Setup(repo => repo.AddAsync(walk)).Returns(Task.CompletedTask);
        _walkRepositoryMock.Setup(repo => repo.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        var result = await _service.ScheduleWalkAsync(walk);

        result.Status.Should().Be(WalkStatus.Scheduled);
    }

    // ✅ CompleteWalkAsync tests
    [Fact]
    public async Task CompleteWalkAsync_ShouldCompleteWalk_WhenWalkerAndInProgress()
    {
        var walk = new Walk { Id = 1, WalkerId = 123, Status = WalkStatus.InProgress };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);
        _walkRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Walk>())).Returns(Task.CompletedTask);

        await _service.CompleteWalkAsync(1, 123);

        walk.Status.Should().Be(WalkStatus.Completed);
    }

    [Fact]
    public async Task CompleteWalkAsync_ShouldThrow_WhenWalkNotFound()
    {
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync((Walk?)null);

        Func<Task> act = async () => await _service.CompleteWalkAsync(1, 123);

        await act.Should().ThrowAsync<Exception>().WithMessage("Walk not found");
    }

    [Fact]
    public async Task CompleteWalkAsync_ShouldThrow_WhenWrongWalker()
    {
        var walk = new Walk { Id = 1, WalkerId = 999, Status = WalkStatus.InProgress };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        Func<Task> act = async () => await _service.CompleteWalkAsync(1, 123);

        await act.Should().ThrowAsync<Exception>().WithMessage("You are not assigned to this walk");
    }

    [Fact]
    public async Task CompleteWalkAsync_ShouldThrow_WhenNotInProgress()
    {
        var walk = new Walk { Id = 1, WalkerId = 123, Status = WalkStatus.Scheduled };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        Func<Task> act = async () => await _service.CompleteWalkAsync(1, 123);

        await act.Should().ThrowAsync<Exception>().WithMessage("Walk is not in progress");
    }

    // ✅ Cancel tests
    [Fact]
    public async Task CancelWalkByWalkerAsync_ShouldCancelWalk()
    {
        var walk = new Walk { Id = 1, WalkerId = 123, Status = WalkStatus.Scheduled };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);
        _walkRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Walk>())).Returns(Task.CompletedTask);

        await _service.CancelWalkByWalkerAsync(1, 123);

        walk.Status.Should().Be(WalkStatus.Canceled);
    }

    [Fact]
    public async Task CancelWalkByOwnerAsync_ShouldCancelWalk()
    {
        var walk = new Walk { Id = 1, OwnerId = 456, Status = WalkStatus.Scheduled };
        _walkRepositoryMock.Setup(r => r.GetWalkByIdAsync(1)).ReturnsAsync(walk);
        _walkRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Walk>())).Returns(Task.CompletedTask);

        await _service.CancelWalkByOwnerAsync(1, 456);

        walk.Status.Should().Be(WalkStatus.Canceled);
    }
}