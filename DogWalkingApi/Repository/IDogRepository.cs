using DogWalkingApi.Models;

namespace DogWalkingApi.Repository;

public interface IDogRepository
{
    Task<List<Dog>> GetDogs();
}
