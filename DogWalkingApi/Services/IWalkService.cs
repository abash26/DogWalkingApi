using DogWalkingApi.DTOs;
using DogWalkingApi.Models;

namespace DogWalkingApi.Services;
public interface IWalkService
{
    Task AcceptWalkAsync(int walkId, int walkerId);
    Task CancelWalkByWalkerAsync(int walkId, int walkerId);
    Task CancelWalkByOwnerAsync(int walkId, int ownerId);
    Task CancelWalk(int walkId);
    Task CompleteWalkAsync(int walkId, int walkerId);
    Task<PagedResult<WalkDto>> GetPendingWalksAsync(int page, int pageSize);
    Task<WalkDto?> GetWalkByIdAsync(int id);
    Task<List<WalkDto>> GetWalksAsync();
    Task<PagedResult<WalkDto>> GetWalksByOwnerIdAsync(
    int ownerId, int page, int pageSize);
    Task<PagedResult<WalkDto>> GetWalksByWalkerIdAsync(int walkerId, int page, int pageSize);
    Task<WalkDto> ScheduleWalkAsync(Walk walk);
    Task StartWalkAsync(int walkId, int walkerId);
}