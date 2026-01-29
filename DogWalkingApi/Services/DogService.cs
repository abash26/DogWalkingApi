using DogWalkingApi.DTOs;
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

    public async Task<Dog?> GetDogByIdAsync(int id)
    {
        return await _dogRepository.GetDogById(id);
    }

    public async Task<Dog> AddDogAsync(CreateDogDTO dog)
    {
        return await _dogRepository.AddDog(dog);
    }

    public async Task<Dog?> UpdateDogAsync(int id, UpdateDogDTO dogDto)
    {
        var existingDog = await _dogRepository.GetDogById(id);
        if (existingDog == null) return null;

        existingDog.Name = dogDto.Name ?? existingDog.Name;
        existingDog.Breed = dogDto.Breed ?? existingDog.Breed;
        existingDog.Age = dogDto.Age ?? existingDog.Age;
        existingDog.Size = dogDto.Size ?? existingDog.Size;
        existingDog.SpecialNeeds = dogDto.SpecialNeeds ?? existingDog.SpecialNeeds;

        return await _dogRepository.UpdateDog(existingDog);
    }

    public async Task<bool> DeleteDogAsync(int id)
    {
        var dog = await _dogRepository.GetDogById(id);
        if (dog == null) return false;

        return await _dogRepository.DeleteDog(dog);
    }
}
