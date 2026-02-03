using DogWalkingApi.Data;
using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DogWalkingApi.Repository;

public class UserRepository : IUserRepository
{
    public readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<User?> AddUser(RegisterDTO user)
    {
        var newUser = new User
        {
            Email = user.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
            Name = user.Name
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }
}
