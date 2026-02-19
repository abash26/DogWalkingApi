using DogWalkingApi.DTOs;
using DogWalkingApi.Models;

namespace DogWalkingApi.Services;
public interface IDogService
{
    Task<Dog> AddDogAsync(CreateDogDTO dto, int ownerId);
    Task<bool> DeleteDogAsync(int id, int ownerId);
    Task<Dog?> GetDogByIdAsync(int id);
    Task<List<Dog>> GetDogsAsync(int ownerId);
    Task<Dog?> UpdateDogAsync(int id, UpdateDogDTO dto, int ownerId);
}