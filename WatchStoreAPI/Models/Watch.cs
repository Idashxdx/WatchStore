using WatchStoreAPI.Models;

public class Watch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public byte[]? Image { get; set; }
    public int TypeId { get; set; }
    public WatchType? Type { get; set; }
}
