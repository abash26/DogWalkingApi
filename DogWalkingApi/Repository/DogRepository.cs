using DogWalkingApi.Data;
using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DogWalkingApi.Repository;

public class DogRepository : IDogRepository
{
    public readonly ApplicationDbContext _context;

    public DogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Dog>> GetDogs()
    {
        return await _context.Dogs.ToListAsync();
    }

    public async Task<Dog?> GetDogById(int id)
    {
        return await _context.Dogs.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Dog> AddDog(CreateDogDTO dog)
    {
        var newDog = new Dog
        {
            Name = dog.Name,
            Breed = dog.Breed,
            Age = dog.Age,
            Size = dog.Size,
            SpecialNeeds = dog.SpecialNeeds,
            OwnerId = dog.OwnerId
        };

        _context.Dogs.Add(newDog);
        await _context.SaveChangesAsync();
        return newDog;
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
