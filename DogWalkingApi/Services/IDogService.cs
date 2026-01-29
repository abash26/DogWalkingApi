using DogWalkingApi.DTOs;
using DogWalkingApi.Models;

namespace DogWalkingApi.Services;

public interface IDogService
{
    Task<List<Dog>> GetDogsAsync();
    Task<Dog?> GetDogByIdAsync(int id);
    Task<Dog> AddDogAsync(CreateDogDTO dog);
    Task<Dog?> UpdateDogAsync(int id, UpdateDogDTO dogDto);
    Task<bool> DeleteDogAsync(int id);
}
