using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchStoreAPI.Data;
using WatchStoreAPI.Models;

namespace WatchStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return BadRequest("Пользователь не найден.");
            }

            var newRequest = new WatchRequest
            {
                UserId = request.UserId,
                Description = request.Description,
                RequestType = request.RequestType,
                StatusId = 1,
                TargetWatchName = request.TargetWatchName,
                TargetBrand = request.TargetBrand,
                TargetPriceRange = request.TargetPriceRange
            };

            _context.WatchRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Заявка успешно создана!" });
        }

        public class CreateRequestRequest
        {
            public int UserId { get; set; }
            public string Description { get; set; } = string.Empty;
            public string RequestType { get; set; } = string.Empty;

            public string? TargetWatchName { get; set; } = null;
            public string? TargetBrand { get; set; } = null;
            public decimal? TargetPriceRange { get; set; } = null;
        }

        // Получение истории заявок
        [HttpGet("{userId}/history")]
        public async Task<IActionResult> GetRequestHistory(int userId)
        {
            var requestHistory = await _context.Set<UserRequest>()
                .FromSqlRaw("SELECT * FROM full_requests_history WHERE user_id = {0}", userId)
                .ToListAsync();

            if (requestHistory == null || !requestHistory.Any())
            {
                return NotFound($"История заявок для пользователя с ID {userId} не найдена.");
            }

            return Ok(requestHistory);
        }
    }
}
