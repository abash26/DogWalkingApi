using DogWalkingApi.Models;

namespace DogWalkingApi.Repository;
public interface IDogRepository
{
    Task<Dog> AddDog(Dog dog);
    Task<bool> DeleteDog(Dog dog);
    Task<Dog?> GetDogById(int id);
    Task<List<Dog>> GetDogs();
    Task<List<Dog>> GetDogsByOwnerId(int ownerId);
    Task<Dog?> UpdateDog(Dog dog);
}