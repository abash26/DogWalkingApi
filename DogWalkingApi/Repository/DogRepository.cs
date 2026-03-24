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

    public async Task<List<Dog>> GetDogs()
    {
        return await _context.Dogs.ToListAsync();
    }

    public async Task<(List<Dog> Items, int TotalCount)> GetDogsByOwnerId(int ownerId, int page, int pageSize)
    {
        var query = _context.Dogs
       .Where(d => d.OwnerId == ownerId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(d => d.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Dog?> GetDogById(int id)
    {
        return await _context.Dogs.FirstOrDefaultAsync(d => d.Id == id);
    }

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
