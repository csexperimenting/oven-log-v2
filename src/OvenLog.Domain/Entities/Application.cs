namespace OvenLog.Domain.Entities;

public class Application
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double? DefaultBakeTime { get; set; }
    public double? DefaultTemperature { get; set; }
    public double? MinTemperature { get; set; }
    public double? MaxTemperature { get; set; }
    public string? Barcode { get; set; }
    
    public ICollection<OvenEvent> Events { get; set; } = new List<OvenEvent>();
}
