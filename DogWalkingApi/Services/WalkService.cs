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

    private static readonly Dictionary<WalkStatus, WalkStatus[]> AllowedTransitions = new()
    {
        { WalkStatus.Pending,    new[] { WalkStatus.Accepted, WalkStatus.Cancelled } },
        { WalkStatus.Accepted,   new[] { WalkStatus.InProgress, WalkStatus.Cancelled } },
        { WalkStatus.InProgress, new[] { WalkStatus.Completed } }
    };

    private static void EnsureTransitionAllowed(Walk walk, WalkStatus newStatus)
    {
        if (!AllowedTransitions.TryGetValue(walk.Status, out var allowed) ||
            !allowed.Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Cannot change walk status from {walk.Status} to {newStatus}");
        }
    }

    private static void EnsureWalker(Walk walk, int walkerId)
    {
        if (walk.WalkerId != walkerId)
            throw new UnauthorizedAccessException("You are not assigned to this walk");
    }

    private static void EnsureOwner(Walk walk, int ownerId)
    {
        if (walk.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You are not the owner of this walk");
    }

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
        walk.Status = WalkStatus.Pending;
        walk.WalkerId = null;

        await _walkRepository.AddAsync(walk);

        var fullWalk = await _walkRepository.GetWalkByIdAsync(walk.Id)
                       ?? throw new KeyNotFoundException("Walk not found after creation");

        return MapToDto(fullWalk);
    }

    public async Task AcceptWalkAsync(int walkId, int walkerId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId)
                   ?? throw new KeyNotFoundException("Walk not found");

        EnsureTransitionAllowed(walk, WalkStatus.Accepted);

        walk.WalkerId = walkerId;
        walk.Status = WalkStatus.Accepted;

        await _walkRepository.UpdateAsync(walk);
    }

    public async Task StartWalkAsync(int walkId, int walkerId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId)
                   ?? throw new KeyNotFoundException("Walk not found");

        EnsureWalker(walk, walkerId);
        EnsureTransitionAllowed(walk, WalkStatus.InProgress);

        walk.Status = WalkStatus.InProgress;

        await _walkRepository.UpdateAsync(walk);
    }

    public async Task CompleteWalkAsync(int walkId, int walkerId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId)
                   ?? throw new KeyNotFoundException("Walk not found");

        EnsureWalker(walk, walkerId);
        EnsureTransitionAllowed(walk, WalkStatus.Completed);

        walk.Status = WalkStatus.Completed;

        await _walkRepository.UpdateAsync(walk);
    }

    public async Task CancelWalkByWalkerAsync(int walkId, int walkerId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId)
                   ?? throw new KeyNotFoundException("Walk not found");

        EnsureWalker(walk, walkerId);
        EnsureTransitionAllowed(walk, WalkStatus.Cancelled);

        walk.Status = WalkStatus.Cancelled;

        await _walkRepository.UpdateAsync(walk);
    }

    public async Task CancelWalkByOwnerAsync(int walkId, int ownerId)
    {
        var walk = await _walkRepository.GetWalkByIdAsync(walkId)
                   ?? throw new KeyNotFoundException("Walk not found");

        EnsureOwner(walk, ownerId);
        EnsureTransitionAllowed(walk, WalkStatus.Cancelled);

        walk.Status = WalkStatus.Cancelled;

        await _walkRepository.UpdateAsync(walk);
    }
}
