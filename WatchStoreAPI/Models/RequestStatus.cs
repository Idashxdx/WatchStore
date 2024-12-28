namespace WatchStoreAPI.Models
{
    public class RequestStatus
    {
        public int Id { get; set; } // Первичный ключ
        public string? Name { get; set; } // Название статуса
        public string? Description { get; set; } // Описание статуса

        // Навигационные свойства
        public ICollection<WatchRequest>? WatchRequests { get; set; }
    }
}
