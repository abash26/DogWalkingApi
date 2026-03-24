using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Repository;

namespace DogWalkingApi.Services;

public class DogService : IDogService
{
    private readonly IDogRepository _dogRepository;

    public DogService(IDogRepository dogRepository)
    {
        _dogRepository = dogRepository;
    }

    public async Task<List<Dog>> GetAllDogsAsync()
    {
        return await _dogRepository.GetDogs();
    }

    // Get all dogs for a specific owner
    public async Task<PagedResult<Dog>> GetDogsAsync(int ownerId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        var (items, totalCount) = await _dogRepository.GetDogsByOwnerId(ownerId, page, pageSize);

        return new PagedResult<Dog>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // Get dog by ID (any user can view)
    public async Task<Dog?> GetDogByIdAsync(int id)
    {
        return await _dogRepository.GetDogById(id);
    }

    // Create dog (Owner only)
    public async Task<Dog> AddDogAsync(CreateDogDTO dto, int ownerId)
    {
        var dog = new Dog
        {
            Name = dto.Name,
            Breed = dto.Breed,
            Age = dto.Age,
            Size = dto.Size,
            SpecialNeeds = dto.SpecialNeeds,
            OwnerId = ownerId // assigned server-side
        };

        return await _dogRepository.AddDog(dog);
    }

    // Update dog (Owner only)
    public async Task<Dog?> UpdateDogAsync(int id, UpdateDogDTO dto, int ownerId)
    {
        var dog = await _dogRepository.GetDogById(id);
        if (dog == null || dog.OwnerId != ownerId)
            return null;

        dog.Name = dto.Name ?? dog.Name;
        dog.Breed = dto.Breed ?? dog.Breed;
        dog.Age = dto.Age ?? dog.Age;
        dog.Size = dto.Size ?? dog.Size;
        dog.SpecialNeeds = dto.SpecialNeeds ?? dog.SpecialNeeds;

        return await _dogRepository.UpdateDog(dog);
    }

    // Delete dog (Owner only)
    public async Task<bool> DeleteDogAsync(int id, int ownerId)
    {
        var dog = await _dogRepository.GetDogById(id);
        if (dog == null || dog.OwnerId != ownerId)
            return false;

        return await _dogRepository.DeleteDog(dog);
    }
}
