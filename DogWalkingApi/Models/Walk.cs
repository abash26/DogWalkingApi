namespace DogWalkingApi.Models;

public class Walk
{
    public int Id { get; set; }
    public DateTime WalkDate { get; set; }
    public TimeSpan Duration { get; set; }
    public string Status { get; set; } // e.g., Scheduled, Completed, Canceled

    // Foreign Keys
    public int DogId { get; set; }
    public int WalkerId { get; set; }
}
