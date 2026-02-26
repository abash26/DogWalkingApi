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

    public async Task<List<WalkDto>> GetPendingWalksAsync()
    {
        var walk = await _walkRepository.GetPendingWalksAsync();
        return walk.Select(MapToDto).ToList();
    }

    public async Task AcceptWalkAsync(int walkId, int walkerId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId)
                   ?? throw new Exception("Walk not found");

        if (walk.Status != WalkStatus.Pending)
            throw new Exception("Walk is not available for acceptance");

        walk.WalkerId = walkerId;
        walk.Status = WalkStatus.Scheduled;

        await _walkRepository.UpdateAsync(walk);
    }

    public async Task StartWalkAsync(int walkId, int walkerId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId)
                   ?? throw new Exception("Walk not found");

        if (walk.WalkerId != walkerId)
            throw new Exception("You are not assigned to this walk");

        if (walk.Status != WalkStatus.Scheduled)
            throw new Exception("Walk is not ready to start");

        walk.Status = WalkStatus.InProgress;

        await _walkRepository.UpdateAsync(walk);
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

    public async Task<WalkDto> ScheduleWalkAsync(Walk walk)
    {
        walk.Status = WalkStatus.Scheduled;
        await _walkRepository.AddAsync(walk);

        var fullWalk = await _walkRepository.GetWalkByIdAsync(walk.Id)
                       ?? throw new Exception("Walk not found after creation");

        return MapToDto(fullWalk);
    }

    public async Task CompleteWalkAsync(int walkId, int walkerId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId)
                   ?? throw new Exception("Walk not found");

        if (walk.WalkerId != walkerId)
            throw new Exception("You are not assigned to this walk");

        if (walk.Status != WalkStatus.InProgress)
            throw new Exception("Walk is not in progress");

        walk.Status = WalkStatus.Completed;

        await _walkRepository.UpdateAsync(walk);
    }

    public async Task CancelWalkByWalkerAsync(int walkId, int walkerId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId)
                   ?? throw new Exception("Walk not found");

        if (walk.WalkerId != walkerId)
            throw new Exception("You are not assigned to this walk");

        if (walk.Status == WalkStatus.Completed)
            throw new Exception("Completed walk cannot be cancelled");

        walk.Status = WalkStatus.Canceled;

        await _walkRepository.UpdateAsync(walk);
    }

    public async Task CancelWalkByOwnerAsync(int walkId, int ownerId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId)
                   ?? throw new Exception("Walk not found");

        if (walk.OwnerId != ownerId)
            throw new Exception("You are not the owner of this walk");

        if (walk.Status == WalkStatus.Completed)
            throw new Exception("Completed walk cannot be cancelled");

        walk.Status = WalkStatus.Canceled;

        await _walkRepository.UpdateAsync(walk);
    }
}
