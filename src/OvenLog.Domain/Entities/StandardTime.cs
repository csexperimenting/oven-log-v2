namespace OvenLog.Domain.Entities;

public class StandardTime
{
    public int Id { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Hours { get; set; }
}
