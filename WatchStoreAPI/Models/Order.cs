using System;

namespace WatchStoreAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string? DeliveryAddress { get; set; }
        public int UserId { get; set; } // FK для  users
        public User? User { get; set; }

        // Связь с часами
        public int WatchId { get; set; } // FK для  watches
        public Watch? Watch { get; set; }
        public int StatusId { get; set; } // FK для  order_statuses
        public OrderStatus? Status { get; set; }
    }
}
