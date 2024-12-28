namespace WatchStoreAPI.Models
{
    public class ArchivedRequestView
    {
        public int RequestId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ArchivedDate { get; set; }
        public string? AssignedEmployee { get; set; }
    }
}