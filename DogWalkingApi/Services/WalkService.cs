using DogWalkingApi.DTOs;
using DogWalkingApi.Models;
using DogWalkingApi.Repository;

namespace DogWalkingApi.Services;

public class WalkService(IWalkRepository walkRepository) : IWalkService
{
    public readonly IWalkRepository _walkRepository = walkRepository;

    private WalkDto MapToDto(Walk w) => new()
    {
        Id = w.Id,
        StartTime = w.StartTime,
        Duration = w.Duration,
        Status = w.Status,
        DogName = w.Dog?.Name ?? "Unknown",
        WalkerName = w.Walker?.Name ?? "Unknown"
    };


    public async Task<List<WalkDto>> GetWalksAsync()
    {
        var walk = await _walkRepository.GetWalksAsync();
        return walk.Select(MapToDto).ToList();
    }

    public async Task<WalkDto?> GetWalkByIdAsync(int id)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(id);
        if (walk == null) return null;
        return MapToDto(walk);
    }

    public async Task<List<WalkDto>> GetWalksByWalkerIdAsync(int walkerId)
    {
        var walk = await _walkRepository.GetWalksByWalkerIdAsync(walkerId);
        return walk.Select(MapToDto).ToList();
    }

    public async Task<List<WalkDto>> GetWalksByOwnerIdAsync(int ownerId)
    {
        var walk = await _walkRepository.GetWalksByOwnerIdAsync(ownerId);
        return walk.Select(MapToDto).ToList();
    }

    // Schedule a walk
    public async Task<WalkDto> ScheduleWalkAsync(Walk walk)
    {
        walk.Status = WalkStatus.Scheduled;
        await _walkRepository.AddAsync(walk);

        var fullWalk = await _walkRepository.GetWalkByIdAsync(walk.Id)
                       ?? throw new Exception("Walk not found after creation");

        return MapToDto(fullWalk);
    }

    // Complete a walk
    public async Task CompleteWalkAsync(int walkId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId) ?? throw new Exception("Walk not found");

        walk.Status = WalkStatus.Completed;

        await _walkRepository.UpdateAsync(walk);
    }

    // Cancel a walk
    public async Task CancelWalkAsync(int walkId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId) ?? throw new Exception("Walk not found");

        walk.Status = WalkStatus.Canceled;

        await _walkRepository.UpdateAsync(walk);
    }
}
