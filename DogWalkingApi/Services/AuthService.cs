using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DogWalkingApi.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    public AuthService(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetUserById(id);
    }

    public async Task<string?> LoginAsync(LoginDTO loginDto)
    {
        var user = await _userRepository.GetUserByEmail(loginDto.Email);
        if (user == null || !IsValidUserCredential(loginDto, user))
        {
            return null;
        }

        return GenerateJwtToken(user);
    }

    public async Task<string?> RegisterAsync(RegisterDTO registerDto)
    {
        var existingUser = await _userRepository.GetUserByEmail(registerDto.Email);
        if (existingUser != null)
        {
            return null;
        }
        var user = await _userRepository.AddUser(registerDto);
        if (user == null)
        {
            return null;
        }
        return GenerateJwtToken(user);
    }

    public bool IsValidUserCredential(LoginDTO loginDto, User user)
    {
        return BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
