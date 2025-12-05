namespace OvenLog.Domain.Entities;

public class OnEvent
{
    public int Id { get; set; }
    
    public int BoxId { get; set; }
    public Box Box { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime IntendedStartTime { get; set; }
    public DateTime ActualRecordedTime { get; set; }
    
    public bool IsWarmingUp(double warmUpMinutes)
    {
        var warmUpEndTime = ActualRecordedTime.AddMinutes(warmUpMinutes);
        return DateTime.UtcNow < warmUpEndTime;
    }
}
