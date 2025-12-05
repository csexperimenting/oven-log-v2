namespace OvenLog.Domain.Entities;

public class Trak
{
    public int Id { get; set; }
    public string TrakId { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string WorkOrder { get; set; } = string.Empty;
    
    public int? PartId { get; set; }
    public Part? Part { get; set; }
    
    public ICollection<OvenEvent> Events { get; set; } = new List<OvenEvent>();
}
