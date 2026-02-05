using DogWalkingApi.Models;

namespace DogWalkingApi.DTOs;

public class WalkDto
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan Duration { get; set; }
    public WalkStatus Status { get; set; }

    public string DogName { get; set; } = null!;
    public string WalkerName { get; set; } = null!;
}
