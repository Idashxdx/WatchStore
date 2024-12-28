public class UserRequest
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public string TargetWatchName { get; set; } = string.Empty;
        public string TargetBrand { get; set; } = string.Empty;
        public decimal? TargetPriceRange { get; set; }
    }