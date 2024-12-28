namespace WatchStoreAPI.Models
{
public class AvailableWatch
{
    public int WatchId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? CaseMaterial { get; set; }
    public string? StrapMaterial { get; set; }
    public decimal? CaseDiameter { get; set; }
    public string? WaterResistance { get; set; }
    public byte[]? Image { get; set; }
}

}
