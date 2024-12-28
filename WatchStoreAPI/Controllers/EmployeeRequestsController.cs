using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WatchStoreAPI.Data;
using WatchStoreAPI.Models;

namespace WatchStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeRequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _context.UserRequests.ToListAsync();
            return Ok(requests);
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] UpdateRequestStatusRequest request)
        {
            if (request == null || request.NewStatusId <= 0 || request.EmployeeId <= 0)
            {
                return BadRequest("Введены некорректные данные.");
            }
            await _context.Database.ExecuteSqlRawAsync(
                "SELECT update_request_status({0}, {1}, {2})",
                id, request.NewStatusId, request.EmployeeId
            );

            return Ok(new { Message = "Статус заявки успешно обновлён!" });
        }
        public class UpdateRequestStatusRequest
        {
            public int NewStatusId { get; set; }
            public int EmployeeId { get; set; }
        }
        [HttpGet("{userId}/employeeId")]
        public async Task<IActionResult> GetEmployeeIdByUserId(int userId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null)
                return NotFound();
            return Ok(employee.Id);
        }
    }
}