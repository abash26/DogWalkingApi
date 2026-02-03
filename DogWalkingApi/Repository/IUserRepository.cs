using DogWalkingApi.DTOs;
using DogWalkingApi.Models;

namespace DogWalkingApi.Repository;
public interface IUserRepository
{
    Task<User?> AddUser(RegisterDTO user);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserById(int id);
}