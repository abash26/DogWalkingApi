using DogWalkingApi.Models;
using DogWalkingApi.Repository;
using DogWalkingApi.Services;
using FluentAssertions;
using Moq;

namespace Tests.Services;
public class WalkServiceTest
{
    public readonly Mock<IWalkRepository> _walkRepositoryMock;
    public readonly IWalkService _service;

    public WalkServiceTest()
    {
        _walkRepositoryMock = new Mock<IWalkRepository>();
        _service = new WalkService(_walkRepositoryMock.Object);
    }

    [Fact]
    public async Task GetWalksAsync_ShouldReturnWalks()
    {
        // Arrange
        var walks = new List<Walk>
        {
            new() { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled },
            new() { Id = 2, StartTime = DateTime.Now.AddHours(1), Duration = TimeSpan.FromMinutes(45), Status = WalkStatus.Scheduled }
        };
        _walkRepositoryMock.Setup(repo => repo.GetWalksAsync()).ReturnsAsync(walks);

        // Act
        var result = await _service.GetWalksAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetWalkByIdAsync_ShouldReturnWalk()
    {
        // Arrange
        var walk = new Walk { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled };
        _walkRepositoryMock.Setup(repo => repo.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        // Act
        var result = await _service.GetWalkByIdAsync(1);

        // Assert
        result.Status.Should().Be(WalkStatus.Scheduled);
    }

    [Fact]
    public async Task GetWalkByIdAsync_ShouldReturnNull_WhenWalkNotFound()
    {
        // Arrange
        _walkRepositoryMock.Setup(repo => repo.GetWalkByIdAsync(1)).ReturnsAsync((Walk?)null);

        // Act
        var result = await _service.GetWalkByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetWalksByWalkerIdAsync_ShouldReturnWalks()
    {
        // Arrange
        var walks = new List<Walk>
        {
            new() { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled, WalkerId = 1 },
            new() { Id = 2, StartTime = DateTime.Now.AddHours(1), Duration = TimeSpan.FromMinutes(45), Status = WalkStatus.Scheduled, WalkerId = 1 }
        };
        _walkRepositoryMock.Setup(repo => repo.GetWalksByWalkerIdAsync(1)).ReturnsAsync(walks);

        // Act
        var result = await _service.GetWalksByWalkerIdAsync(1);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetWalksByWalkerIdAsync_ShouldReturnEmptyList_WhenNoWalksFound()
    {
        // Arrange
        _walkRepositoryMock.Setup(repo => repo.GetWalksByWalkerIdAsync(1)).ReturnsAsync([]);

        // Act
        var result = await _service.GetWalksByWalkerIdAsync(1);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetWalksByOwnerIdAsync_ShouldReturnWalks()
    {
        // Arrange
        var walks = new List<Walk>
        {
            new() { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled },
            new() { Id = 2, StartTime = DateTime.Now.AddHours(1), Duration = TimeSpan.FromMinutes(45), Status = WalkStatus.Scheduled }
        };
        _walkRepositoryMock.Setup(repo => repo.GetWalksByOwnerIdAsync(1)).ReturnsAsync(walks);

        // Act
        var result = await _service.GetWalksByOwnerIdAsync(1);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task ScheduleWalkAsync_ShouldScheduleWalk()
    {
        // Arrange
        var walk = new Walk { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30) };
        _walkRepositoryMock.Setup(repo => repo.AddAsync(walk)).Returns(Task.CompletedTask);
        _walkRepositoryMock.Setup(repo => repo.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        // Act
        var result = await _service.ScheduleWalkAsync(walk);

        // Assert
        result.Status.Should().Be(WalkStatus.Scheduled);
    }

    [Fact]
    public async Task CompleteWalkAsync_ShouldCompleteWalk()
    {
        // Arrange
        var walk = new Walk { Id = 1, StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(30), Status = WalkStatus.Scheduled };
        _walkRepositoryMock.Setup(repo => repo.GetWalkByIdAsync(1)).ReturnsAsync(walk);
        _walkRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Walk>())).Returns(Task.CompletedTask);

        // Act
        await _service.CompleteWalkAsync(1);

        // Assert
        walk.Status.Should().Be(WalkStatus.Completed);
    }

    [Fact]
    public async Task CancelWalkAsync_ShouldCancelWalk()
    {
        // Arrange
        var walk = new Walk
        {
            Id = 1,
            StartTime = DateTime.Now,
            Duration = TimeSpan.FromMinutes(30),
            Status = WalkStatus.Scheduled
        };

        _walkRepositoryMock.Setup(repo => repo.GetWalkByIdAsync(1)).ReturnsAsync(walk);

        // Act
        await _service.CancelWalkAsync(1);

        // Assert
        walk.Status.Should().Be(WalkStatus.Canceled);
    }
}
