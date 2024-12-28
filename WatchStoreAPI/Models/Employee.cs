using System;

namespace WatchStoreAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string? Position { get; set; }
        public DateTime HireDate { get; set; }


        public int UserId { get; set; } // FK для users
        public User? User { get; set; }
    }
}
