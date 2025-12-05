namespace OvenLog.Domain.Entities;

public class Box
{
    public int Id { get; set; }
    public string ToolId { get; set; } = string.Empty;
    public double DefaultTemperature { get; set; }
    public string TemperatureScale { get; set; } = "C";
    public double? WarmUpTime { get; set; }
    public bool HasDigitalDisplay { get; set; } = true;
    
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
    
    public int ModelId { get; set; }
    public Model Model { get; set; } = null!;
    
    public int BoxTypeId { get; set; }
    public BoxType BoxType { get; set; } = null!;
    
    public ICollection<OvenEvent> Events { get; set; } = new List<OvenEvent>();
    public ICollection<OnEvent> OnEvents { get; set; } = new List<OnEvent>();
}
