using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WatchStoreAPI.Data;

namespace WatchStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeOrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeOrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.UserOrders.ToListAsync();
            return Ok(orders);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            if (request == null || request.NewStatusId <= 0 || request.EmployeeId <= 0)
            {
                return BadRequest("Переданы некорректные данные.");
            }

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "SELECT update_order_status({0}, {1}, {2})",
                    id, request.NewStatusId, request.EmployeeId
                );

                return Ok(new { Message = "Статус заказа успешно обновлён!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        public class UpdateOrderStatusRequest
        {
            public int NewStatusId { get; set; }
            public int EmployeeId { get; set; }
        }
    }
}
