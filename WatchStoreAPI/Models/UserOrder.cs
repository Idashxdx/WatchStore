public class UserOrder
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
}
