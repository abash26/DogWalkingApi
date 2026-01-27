using DogWalkingApi.Models;

namespace DogWalkingApi.Services;

public interface IDogService
{
    Task<List<Dog>> GetDogsAsync();

}
