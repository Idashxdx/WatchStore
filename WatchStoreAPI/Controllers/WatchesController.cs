using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchStoreAPI.Data;
using WatchStoreAPI.Models;

namespace WatchStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WatchesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableWatches(string? filter = null,string? sortOption = null)
        {
            IQueryable<AvailableWatch> query = _context.AvailableWatches;

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(w => EF.Functions.ILike(w.ModelName, $"%{filter}%"));
            }

            if (!string.IsNullOrEmpty(sortOption))
            {
                query = sortOption.ToLower() switch
                {
                    "asc" => query.OrderBy(w => w.Price),
                    "desc" => query.OrderByDescending(w => w.Price),
                    _ => query
                };
            }

            var watches = await query.ToListAsync();

            return Ok(watches);
        }

    }
}
