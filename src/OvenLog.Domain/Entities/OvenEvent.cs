namespace OvenLog.Domain.Entities;

public class OvenEvent
{
    public int Id { get; set; }
    
    public int BoxId { get; set; }
    public Box Box { get; set; } = null!;
    
    public int UserInId { get; set; }
    public User UserIn { get; set; } = null!;
    
    public DateTime DateIn { get; set; }
    
    public int? UserOutId { get; set; }
    public User? UserOut { get; set; }
    
    public DateTime? DateOut { get; set; }
    
    public int TrakId { get; set; }
    public Trak Trak { get; set; } = null!;
    
    public string? Note { get; set; }
    public int Quantity { get; set; } = 1;
    
    public int? ApplicationId { get; set; }
    public Application? Application { get; set; }
    
    public double BakeTimeHours { get; set; }
    public double Temperature { get; set; }
    
    public TimeSpan? TimeRemaining
    {
        get
        {
            if (DateOut.HasValue) return TimeSpan.Zero;
            var endTime = DateIn.AddHours(BakeTimeHours);
            var remaining = endTime - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }
    
    public TimeSpan ActualBakeTime
    {
        get
        {
            var endTime = DateOut ?? DateTime.UtcNow;
            return endTime - DateIn;
        }
    }
    
    public bool IsInOven => !DateOut.HasValue;
}
