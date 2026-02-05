namespace DogWalkingApi.DTOs;

public class WalkCreateDto
{
    public DateTime StartTime { get; set; }
    public TimeSpan Duration { get; set; }
    public int DogId { get; set; }
    public int WalkerId { get; set; }
}
