namespace WatchStoreAPI.Models
{
    public class WatchType
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public ICollection<Watch>? Watches { get; set; }
    }
}
