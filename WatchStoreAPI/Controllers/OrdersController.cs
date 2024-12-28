using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchStoreAPI.Data;
using WatchStoreAPI.Models;

namespace WatchStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
           var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return BadRequest("Пользователь не найден.");

            var watch = await _context.Watches.FindAsync(request.WatchId);
            if (watch == null || watch.Quantity < request.Quantity)
                return BadRequest("Недостаточное количество товара.");

            var order = new Order
            {
                UserId = request.UserId,
                WatchId = request.WatchId,
                Quantity = request.Quantity,
                DeliveryAddress = request.DeliveryAddress,
                OrderDate = DateTime.UtcNow,
                StatusId = 1
            };

            _context.Orders.Add(order);
            watch.Quantity -= request.Quantity;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Заказ успешно оформлен!" });
        }

        [HttpGet("{userId}/history")]
        public async Task<IActionResult> GetUserOrderHistory(int userId)
        {
            var orderHistory = await _context.Set<UserOrder>()
                .FromSqlRaw("SELECT * FROM full_orders_history WHERE user_id = {0}", userId)
                .ToListAsync();

            if (orderHistory == null || !orderHistory.Any())
            {
                return NotFound($"История заказов для пользователя с ID {userId} не найдена.");
            }

            return Ok(orderHistory);
        }



    }

    public class CreateOrderRequest
    {
        public int UserId { get; set; }
        public int WatchId { get; set; }
        public int Quantity { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
    }


}
