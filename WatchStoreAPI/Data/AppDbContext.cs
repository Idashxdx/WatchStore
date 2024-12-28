using Microsoft.EntityFrameworkCore;
using WatchStoreAPI.Controllers;
using WatchStoreAPI.Models;

namespace WatchStoreAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<WatchType> WatchTypes { get; set; }
        public DbSet<Watch> Watches { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<RequestStatus> RequestStatuses { get; set; }
        public DbSet<WatchRequest> WatchRequests { get; set; }
        public DbSet<AvailableWatch> AvailableWatches { get; set; }
        public DbSet<UserOrder> UserOrders { get; set; }
        public DbSet<UserRequest> UserRequests { get; set; }
        public DbSet<ArchivedRequestView> ArchivedRequestsView { get; set; } = null!;
        public DbSet<ArchivedOrderView> ArchivedOrdersView { get; set; } = null!;
        public DbSet<EmployeeClientsController.ClientUser> ClientUsers { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Меняем имена таблиц
                entity.SetTableName(ConvertToSnakeCase(entity.GetTableName()));

                // Меняем имена столбцов
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ConvertToSnakeCase(property.Name));
                }
            }
            modelBuilder.Entity<UserOrder>().ToView("full_orders_history").HasNoKey();
            modelBuilder.Entity<UserRequest>().ToView("full_requests_history").HasNoKey();
            modelBuilder.Entity<AvailableWatch>().ToView("available_watches").HasNoKey();
            modelBuilder.Entity<UserOrder>().ToView("user_orders").HasNoKey();
            modelBuilder.Entity<UserRequest>().ToView("user_requests").HasNoKey();
            modelBuilder.Entity<ArchivedRequestView>().ToView("archived_requests_view").HasNoKey();
            modelBuilder.Entity<WatchRequest>(entity =>
                {
                    entity.Property(wr => wr.CreationDate)
                          .HasDefaultValueSql("CURRENT_TIMESTAMP");
                });
            modelBuilder.Entity<Employee>().Property(e => e.HireDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<ArchivedOrderView>().ToView("archived_orders_view").HasNoKey();
            modelBuilder.Entity<EmployeeClientsController.ClientUser>().ToView("client_users").HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
        private string ConvertToSnakeCase(string input)
        {
            return string.Concat(
                input.Select((x, i) =>
                    i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()
                )
            ).ToLower();
        }
    }
}
