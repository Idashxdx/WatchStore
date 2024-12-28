using System;

namespace WatchStoreAPI.Models
{
    public class WatchRequest
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? RequestType { get; set; }
        public DateTime CreationDate { get; set; }
        public string? TargetWatchName { get; set; }
        public string? TargetBrand { get; set; }
        public decimal? TargetPriceRange { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public int StatusId { get; set; }
        public RequestStatus? Status { get; set; }
    }
}
