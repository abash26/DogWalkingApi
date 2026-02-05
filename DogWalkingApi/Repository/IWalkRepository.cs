using DogWalkingApi.Models;

namespace DogWalkingApi.Repository;
public interface IWalkRepository
{
    Task<Walk?> GetWalkByIdAsync(int id);
    Task<List<Walk>> GetWalksAsync();
    Task<List<Walk>> GetWalksByOwnerIdAsync(int ownerId);
    Task<List<Walk>> GetWalksByWalkerIdAsync(int walkerId);
    Task AddAsync(Walk walk);
    Task UpdateAsync(Walk walk);
}