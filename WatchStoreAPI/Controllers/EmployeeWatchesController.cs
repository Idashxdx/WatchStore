using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WatchStoreAPI.Data;
using WatchStoreAPI.Models;

namespace WatchStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeWatchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeWatchesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWatches()
        {
            var watches = await _context.AvailableWatches.ToListAsync();
            return Ok(watches);
        }

        [HttpPost]
        public async Task<IActionResult> AddWatch([FromBody] AddWatchRequest request)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "SELECT add_watch({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                request.Name, request.Brand, request.TypeId, request.Price, request.Quantity, request.CaseMaterial,
                request.StrapMaterial, request.CaseDiameter, request.WaterResistance);

            return Ok(new { Message = "Товар успешно добавлен!" });
        }

        // 3. Удаление товара
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveWatch(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT delete_watch({0})", id);
            return Ok(new { Message = "Товар успешно удалён!" });
        }
        [HttpPost("upload-image/{watchId}")]
        public async Task<IActionResult> UploadImage(int watchId, IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Файл изображения обязателен.");
            if (!image.ContentType.StartsWith("image/"))
                return BadRequest("Файл должен быть изображением.");
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                var watch = await _context.Watches.FindAsync(watchId);
                if (watch == null)
                    return NotFound("Часы не найдены.");
                watch.Image = imageBytes;
                await _context.SaveChangesAsync();
            }
            return Ok("Изображение успешно загружено.");
        }
        [HttpGet("{watchId}/image")]
        public async Task<IActionResult> GetImage(int watchId)
        {
            var watch = await _context.Watches.FindAsync(watchId);
            if (watch != null && watch.Image != null)
                return File(watch.Image, "image/jpeg");

            return NotFound("Изображение не найдено.");
        }
        public class AddWatchRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Brand { get; set; } = string.Empty;
            public int TypeId { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public string? CaseMaterial { get; set; }
            public string? StrapMaterial { get; set; }
            public decimal? CaseDiameter { get; set; }
            public string? WaterResistance { get; set; }
        }
    }
}
