namespace DogWalkingApi.Models;
public enum WalkStatus
{
    Scheduled,
    Completed,
    Canceled
}

public class Walk
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan Duration { get; set; }
    public WalkStatus Status { get; set; }

    // Foreign Keys
    public int DogId { get; set; }
    public int WalkerId { get; set; }

    // Navigation properties
    public Dog? Dog { get; set; }
    public User? Walker { get; set; }
}
