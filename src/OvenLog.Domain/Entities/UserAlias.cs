namespace OvenLog.Domain.Entities;

public class UserAlias
{
    public int Id { get; set; }
    public string Alias { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
