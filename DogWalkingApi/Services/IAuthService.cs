using DogWalkingApi.DTOs;
using DogWalkingApi.Models;

namespace DogWalkingApi.Services;
public interface IAuthService
{
    string GenerateJwtToken(User user);
    Task<User?> GetUserByIdAsync(int id);
    bool IsValidUserCredential(LoginDTO loginDto, User user);
    Task<string?> LoginAsync(LoginDTO loginDto);
    Task<string?> RegisterAsync(RegisterDTO registerDto);
}