using DogWalkingApi.Models;
using DogWalkingApi.Repository;

namespace DogWalkingApi.Services;

public class DogService : IDogService
{
    public readonly IDogRepository _dogRepository;
    public DogService(IDogRepository dogRepository)
    {
        _dogRepository = dogRepository;
    }

    public async Task<List<Dog>> GetDogsAsync()
    {
        return await _dogRepository.GetDogs();
    }
}
