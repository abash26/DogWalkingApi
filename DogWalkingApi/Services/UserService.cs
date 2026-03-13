using DogWalkingApi.Models;
using DogWalkingApi.Repository;

namespace DogWalkingApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsers();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetUserById(id);
    }

    public async Task<bool> UpdateUserRoleAsync(int userId, UserRole role)
    {
        var user = await _userRepository.GetUserById(userId);

        if (user == null)
            return false;

        user.Role = role;

        await _userRepository.UpdateUserAsync(user);

        return true;
    }
}
