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

    public async Task<(List<Walk> Items, int TotalCount)> GetPendingWalksAsync(int page, int pageSize)
    {
        var now = DateTime.UtcNow;

        var query = _context.Walks
            .Include(w => w.Dog)
            .Include(w => w.Walker)
            .Where(w => w.Status == WalkStatus.Pending && w.StartTime > now);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(w => w.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Walk?> GetWalkByIdAsync(int id)
    {
        return await _context.Walks.Include(w => w.Dog)
            .Include(w => w.Walker)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<(List<Walk> Items, int TotalCount)> GetWalksByWalkerIdAsync(int walkerId, int page, int pageSize)
    {
        var query = _context.Walks
            .Include(w => w.Dog)
            .Include(w => w.Walker)
            .Where(w => w.WalkerId == walkerId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(w => w.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<Walk> Items, int TotalCount)> GetWalksByOwnerIdAsync(
    int ownerId, int page, int pageSize)
    {
        var query = _context.Walks
            .Include(w => w.Dog)
            .Include(w => w.Walker)
            .Where(w => w.Dog.OwnerId == ownerId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(w => w.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
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

    public async Task<bool> AcceptWalkAsync(int walkId, int walkerId)
    {
        var rows = await _context.Database.ExecuteSqlInterpolatedAsync($@"
        UPDATE Walks
        SET WalkerId = {walkerId}, Status = {(int)WalkStatus.Accepted}
        WHERE Id = {walkId} AND Status = {(int)WalkStatus.Pending}
    ");

        return rows > 0;
    }
}
