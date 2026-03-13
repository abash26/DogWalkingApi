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
    Task<List<WalkDto>> GetPendingWalksAsync();
    Task<WalkDto?> GetWalkByIdAsync(int id);
    Task<List<WalkDto>> GetWalksAsync();
    Task<List<WalkDto>> GetWalksByOwnerIdAsync(int ownerId);
    Task<List<WalkDto>> GetWalksByWalkerIdAsync(int walkerId);
    Task<WalkDto> ScheduleWalkAsync(Walk walk);
    Task StartWalkAsync(int walkId, int walkerId);
}