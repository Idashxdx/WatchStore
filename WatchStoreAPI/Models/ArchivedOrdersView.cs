namespace WatchStoreAPI.Models
{
    public class ArchivedOrderView
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string WatchName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime ArchivedDate { get; set; } // Дата архивации
        public string DeliveryAddress { get; set; } = string.Empty;
    }
}

