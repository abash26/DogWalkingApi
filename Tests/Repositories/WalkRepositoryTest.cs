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
        await TestHelpers.AddTestWalk(_context, ownerId: 10, walkerId: 5);

        var walks = await _repository.GetWalksAsync();

        walks.Should().HaveCount(1);
        walks[0].Dog.Should().NotBeNull();
        walks[0].Walker.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWalkByIdAsync_ReturnsWalkWithDogAndWalker()
    {
        var walk = await TestHelpers.AddTestWalk(_context, ownerId: 10, walkerId: 5);
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
        // Create owner and walkers
        var owner = await TestHelpers.AddTestUser(_context, role: UserRole.Owner);
        var walker5 = await TestHelpers.AddTestUser(_context, role: UserRole.Walker);
        var walker6 = await TestHelpers.AddTestUser(_context, role: UserRole.Walker);

        // Add walks with correct IDs
        await TestHelpers.AddTestWalk(_context, owner.Id, walker5.Id);
        await TestHelpers.AddTestWalk(_context, owner.Id, walker6.Id);

        // Fetch walks for walker5
        var walks = await _repository.GetWalksByWalkerIdAsync(walker5.Id, page: 1, pageSize: 10);

        walks.Items.Should().HaveCount(1);
        walks.Items[0].WalkerId.Should().Be(walker5.Id);
    }

    [Fact]
    public async Task GetWalksByOwnerIdAsync_ReturnsOnlyWalksForOwner()
    {
        await TestHelpers.AddTestWalk(_context, ownerId: 10, walkerId: 5);
        await TestHelpers.AddTestWalk(_context, ownerId: 20, walkerId: 6);

        var pagedResult = await _repository.GetWalksByOwnerIdAsync(ownerId: 10, page: 1, pageSize: 10);

        pagedResult.Items.Should().HaveCount(1);
        pagedResult.Items[0].OwnerId.Should().Be(10);
        pagedResult.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task AddAsync_PersistsWalk()
    {
        var dog = await TestHelpers.AddTestDog(_context, ownerId: 1);
        var walker = await TestHelpers.AddTestUser(_context);

        var walk = new Walk
        {
            DogId = dog.Id,
            OwnerId = dog.OwnerId,  // assign owner FK
            WalkerId = walker.Id,   // assign walker FK
            StartTime = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(1),
            Status = WalkStatus.Pending,
            Dog = dog,
            Walker = walker
        };

        await _repository.AddAsync(walk);

        var persisted = await _context.Walks.FindAsync(walk.Id);
        persisted.Should().NotBeNull();
        persisted.WalkerId.Should().Be(walker.Id);
        persisted.OwnerId.Should().Be(dog.OwnerId);
        persisted.DogId.Should().Be(dog.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesWalk()
    {
        var walk = await TestHelpers.AddTestWalk(_context, ownerId: 1, walkerId: 2);

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

    [Fact]
    public async Task AcceptWalkAsync_WhenWalkIsPending_ShouldAssignWalkerAndUpdateStatus()
    {
        var dog = await TestHelpers.AddTestDog(_context, ownerId: 1);

        var walk = new Walk
        {
            DogId = dog.Id,
            OwnerId = dog.OwnerId,
            StartTime = DateTime.UtcNow,
            Duration = TimeSpan.FromMinutes(30),
            Status = WalkStatus.Pending,
            WalkerId = null
        };

        _context.Walks.Add(walk);
        await _context.SaveChangesAsync();

        var walker = await TestHelpers.AddTestUser(_context, UserRole.Walker);

        var success = await _repository.AcceptWalkAsync(walk.Id, walker.Id);

        success.Should().BeTrue();

        // IMPORTANT: reload entity because raw SQL bypasses EF tracking
        await _context.Entry(walk).ReloadAsync();

        walk.WalkerId.Should().Be(walker.Id);
        walk.Status.Should().Be(WalkStatus.Accepted);
    }

    [Fact]
    public async Task AcceptWalkAsync_WhenWalkAlreadyAccepted_ShouldReturnFalse()
    {
        var walk = await TestHelpers.AddTestWalk(_context, ownerId: 1, walkerId: 2);
        walk.Status = WalkStatus.Accepted;
        await _context.SaveChangesAsync();

        var success = await _repository.AcceptWalkAsync(walk.Id, 99);

        success.Should().BeFalse();
    }

    [Fact]
    public async Task AcceptWalkAsync_WhenWalkDoesNotExist_ShouldReturnFalse()
    {
        var success = await _repository.AcceptWalkAsync(999, 1);

        success.Should().BeFalse();
    }
}