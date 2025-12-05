namespace OvenLog.Domain.Entities;

public class UserOvenSelection
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int BoxId { get; set; }
    public Box Box { get; set; } = null!;
}
