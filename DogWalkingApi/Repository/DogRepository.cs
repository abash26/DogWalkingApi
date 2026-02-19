using DogWalkingApi.Data;
using DogWalkingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DogWalkingApi.Repository;

public class DogRepository : IDogRepository
{
    private readonly ApplicationDbContext _context;

    public DogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Get all dogs (optionally filtered by owner)
    public async Task<List<Dog>> GetDogs()
    {
        return await _context.Dogs.ToListAsync();
    }

    // Get dogs for a specific owner
    public async Task<List<Dog>> GetDogsByOwnerId(int ownerId)
    {
        return await _context.Dogs
            .Where(d => d.OwnerId == ownerId)
            .ToListAsync();
    }

    // Get single dog by ID
    public async Task<Dog?> GetDogById(int id)
    {
        return await _context.Dogs.FirstOrDefaultAsync(d => d.Id == id);
    }

    // Add dog (ownerId must come from server, not client)
    public async Task<Dog> AddDog(Dog dog)
    {
        _context.Dogs.Add(dog);
        await _context.SaveChangesAsync();
        return dog;
    }

    public async Task<Dog?> UpdateDog(Dog dog)
    {
        var existingDog = await GetDogById(dog.Id);
        if (existingDog == null) return null;

        existingDog.Name = dog.Name;
        existingDog.Breed = dog.Breed;
        existingDog.Age = dog.Age;
        existingDog.Size = dog.Size;
        existingDog.SpecialNeeds = dog.SpecialNeeds;

        await _context.SaveChangesAsync();
        return existingDog;
    }

    public async Task<bool> DeleteDog(Dog dog)
    {
        _context.Dogs.Remove(dog);
        await _context.SaveChangesAsync();
        return true;
    }
}
