using DogWalkingApi.Data;
using DogWalkingApi.Models;
using DogWalkingApi.Repository;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;

namespace Tests.Repositories;
public class WalkRepositoryTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;
    private readonly WalkRepository _repository;

    public WalkRepositoryTest()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options, seedData: false);
        _context.Database.EnsureCreated();

        _repository = new WalkRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task GetWalksAsync_ReturnsAllWalks_WithDogAndWalker()
    {
        await TestHelpers.AddTestWalk(_context, ownerId: 10);

        var walks = await _repository.GetWalksAsync();

        walks.Should().HaveCount(1);
        walks[0].Dog.Should().NotBeNull();
        walks[0].Walker.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWalkByIdAsync_ReturnsWalkWithDogAndWalker()
    {
        var walk = await TestHelpers.AddTestWalk(_context, ownerId: 10);
        var result = await _repository.GetWalkByIdAsync(walk.Id);

        result.Should().NotBeNull();
        result?.Dog.Should().NotBeNull();
        result?.Walker.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWalkByIdAsync_WhenWalkDoesNotExist_ShouldReturnNull()
    {
        var result = await _repository.GetWalkByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetWalksByWalkerIdAsync_ReturnsWalksForWalker()
    {
        var walk1 = await TestHelpers.AddTestWalk(_context, ownerId: 10);
        await TestHelpers.AddTestWalk(_context, ownerId: 20);

        var walkerId = walk1.WalkerId;

        var walks = await _repository.GetWalksByWalkerIdAsync(walkerId);

        walks.Should().HaveCount(1);
        walks[0].WalkerId.Should().Be(walkerId);
    }

    [Fact]
    public async Task GetWalksByOwnerIdAsync_ReturnsOnlyWalksForOwner()
    {
        await TestHelpers.AddTestWalk(_context, ownerId: 10);
        await TestHelpers.AddTestWalk(_context, ownerId: 20);

        var walks = await _repository.GetWalksByOwnerIdAsync(10);

        walks.Should().HaveCount(1);
        walks[0].Dog.OwnerId.Should().Be(10);
    }

    [Fact]
    public async Task AddAsync_PersistsWalk()
    {
        var dog = await TestHelpers.AddTestDog(_context, ownerId: 1);
        var walker = await TestHelpers.AddTestUser(_context);

        var walk = new Walk
        {
            DogId = dog.Id,
            WalkerId = walker.Id,
            StartTime = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(1),
            Status = WalkStatus.Scheduled
        };

        await _repository.AddAsync(walk);

        var persisted = await _context.Walks.FindAsync(walk.Id);
        persisted.Should().NotBeNull();
        persisted.WalkerId.Should().Be(walker.Id);
        persisted.DogId.Should().Be(dog.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesWalk()
    {
        var walk = await TestHelpers.AddTestWalk(_context, ownerId: 1);

        var newStartTime = DateTime.UtcNow.AddDays(1);
        var newDuration = TimeSpan.FromHours(2);
        var newStatus = WalkStatus.Completed;

        walk.StartTime = newStartTime;
        walk.Duration = newDuration;
        walk.Status = newStatus;

        await _repository.UpdateAsync(walk);

        var updated = await _context.Walks.FindAsync(walk.Id);
        updated!.StartTime.Should().Be(newStartTime);
        updated.Duration.Should().Be(newDuration);
        updated.Status.Should().Be(newStatus);
    }
}
