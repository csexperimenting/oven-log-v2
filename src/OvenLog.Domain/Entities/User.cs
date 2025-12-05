namespace OvenLog.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public int? DedicatedBoxId { get; set; }
    public Box? DedicatedBox { get; set; }
    
    public ICollection<UserAlias> Aliases { get; set; } = new List<UserAlias>();
    public ICollection<OvenEvent> EventsIn { get; set; } = new List<OvenEvent>();
    public ICollection<OvenEvent> EventsOut { get; set; } = new List<OvenEvent>();
    public ICollection<OnEvent> OnEvents { get; set; } = new List<OnEvent>();
    
    public string FullName => string.IsNullOrEmpty(MiddleName) 
        ? $"{FirstName} {LastName}" 
        : $"{FirstName} {MiddleName} {LastName}";
}
