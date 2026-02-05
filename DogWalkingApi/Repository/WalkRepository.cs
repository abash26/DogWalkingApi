using DogWalkingApi.Data;
using DogWalkingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DogWalkingApi.Repository;

public class WalkRepository(ApplicationDbContext context) : IWalkRepository
{
    public readonly ApplicationDbContext _context = context;

    public async Task<List<Walk>> GetWalksAsync()
    {
        return await _context.Walks.Include(w => w.Dog)
        .Include(w => w.Walker).ToListAsync();
    }

    public async Task<Walk?> GetWalkByIdAsync(int id)
    {
        return await _context.Walks.Include(w => w.Dog)
            .Include(w => w.Walker)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<List<Walk>> GetWalksByWalkerIdAsync(int walkerId)
    {
        return await _context.Walks.Include(w => w.Dog)
                             .Include(w => w.Walker)
                             .Where(w => w.WalkerId == walkerId)
                             .ToListAsync();
    }

    public async Task<List<Walk>> GetWalksByOwnerIdAsync(int ownerId)
    {
        return await _context.Walks.Include(w => w.Dog)
                             .Include(w => w.Walker)
                             .Where(w => w.Dog.OwnerId == ownerId)
                             .ToListAsync();
    }

    public async Task AddAsync(Walk walk)
    {
        _context.Walks.Add(walk);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Walk walk)
    {
        _context.Walks.Update(walk);
        await _context.SaveChangesAsync();
    }
}
