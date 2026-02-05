using DogWalkingApi.DTOs;
using DogWalkingApi.Models;

namespace DogWalkingApi.Services;
public interface IWalkService
{
    Task<WalkDto?> GetWalkByIdAsync(int id);
    Task<List<WalkDto>> GetWalksAsync();
    Task<List<WalkDto>> GetWalksByOwnerIdAsync(int ownerId);
    Task<List<WalkDto>> GetWalksByWalkerIdAsync(int walkerId);
    Task<WalkDto> ScheduleWalkAsync(Walk walk);
    Task CompleteWalkAsync(int walkId);
    Task CancelWalkAsync(int walkId);
}