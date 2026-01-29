using DogWalkingApi.DTOs;
using DogWalkingApi.Models;

namespace DogWalkingApi.Repository;

public interface IDogRepository
{
    Task<List<Dog>> GetDogs();
    Task<Dog?> GetDogById(int id);

    Task<Dog> AddDog(CreateDogDTO dog);
    Task<Dog?> UpdateDog(Dog dog);
    Task<bool> DeleteDog(Dog dog);
}
