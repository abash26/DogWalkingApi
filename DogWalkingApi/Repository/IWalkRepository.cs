using DogWalkingApi.Models;

namespace DogWalkingApi.Repository;
public interface IWalkRepository
{
    Task<Walk?> GetWalkByIdAsync(int id);
    Task<List<Walk>> GetWalksAsync();
    Task<(List<Walk> Items, int TotalCount)> GetPendingWalksAsync(int page, int pageSize);
    Task<(List<Walk> Items, int TotalCount)> GetWalksByOwnerIdAsync(
    int ownerId, int page, int pageSize);
    Task<(List<Walk> Items, int TotalCount)> GetWalksByWalkerIdAsync(int walkerId, int page, int pageSize);
    Task AddAsync(Walk walk);
    Task UpdateAsync(Walk walk);
    Task<bool> AcceptWalkAsync(int walkId, int walkerId);
}