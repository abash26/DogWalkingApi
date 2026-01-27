using DogWalkingApi.Data;
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
}
