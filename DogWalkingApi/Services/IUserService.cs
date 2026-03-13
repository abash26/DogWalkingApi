using DogWalkingApi.Models;

namespace DogWalkingApi.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<bool> UpdateUserRoleAsync(int userId, UserRole role);
}