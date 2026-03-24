using DogWalkingApi.Models;

namespace DogWalkingApi.Repository;
public interface IDogRepository
{
    Task<Dog> AddDog(Dog dog);
    Task<bool> DeleteDog(Dog dog);
    Task<Dog?> GetDogById(int id);
    Task<List<Dog>> GetDogs();
    Task<(List<Dog> Items, int TotalCount)> GetDogsByOwnerId(int ownerId, int page, int pageSize);
    Task<Dog?> UpdateDog(Dog dog);
}