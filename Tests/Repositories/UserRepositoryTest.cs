using DogWalkingApi.Data;
using DogWalkingApi.DTOs;
using DogWalkingApi.Repository;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;

namespace Tests.Repositories;
public class UserRepositoryTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTest()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options, seedData: false);
        _context.Database.EnsureCreated();

        _repository = new UserRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser()
    {
        var user = await TestHelpers.AddTestUser(_context, "test@test.com", "Test User");

        var result = await _repository.GetUserById(user.Id);

        result.Should().NotBeNull();
        result?.Email.Should().Be("test@test.com");
        result?.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetUserById_WhenUserDoesNotExist_ShouldReturnNull()
    {
        var result = await _repository.GetUserById(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByEmail_ShouldReturnUser()
    {
        await TestHelpers.AddTestUser(_context, "test@test.com", "Test User");

        var result = await _repository.GetUserByEmail("test@test.com");

        result.Should().NotBeNull();
        result?.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetUserByEmail_WhenUserDoesNotExist_ShouldReturnNull()
    {
        var result = await _repository.GetUserByEmail("missing@test.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddUser_ShouldCreateUserWithHashedPassword()
    {
        var dto = new RegisterDTO
        {
            Email = "new@test.com",
            Password = "PlainTextPassword123!",
            Name = "New User"
        };

        var user = await _repository.AddUser(dto);

        user.Should().NotBeNull();
        user.Email.Should().Be("new@test.com");
        user.Name.Should().Be("New User");

        // Password should NOT be stored as plain text
        user.PasswordHash.Should().NotBe(dto.Password);

        // Verify hash matches password
        BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)
            .Should().BeTrue();
    }
}
