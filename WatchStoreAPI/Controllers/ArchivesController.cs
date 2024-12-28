using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using WatchStoreAPI.Data;

namespace WatchStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArchivesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArchivesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetArchivedRequests(int? userId = null, DateTime? fromDate = null, DateTime? toDate = null, bool descending = false)
        {
            var query = _context.ArchivedRequestsView.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(r => r.ClientId == userId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(r => r.ArchivedDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(r => r.ArchivedDate <= toDate.Value);
            }

            query = descending
                ? query.OrderByDescending(r => r.ArchivedDate)
                : query.OrderBy(r => r.ArchivedDate);

            var archivedRequests = await query.ToListAsync();
            return Ok(archivedRequests);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetArchivedOrders(int? userId = null, DateTime? fromDate = null, DateTime? toDate = null, bool descending = false)
        {
            var query = _context.ArchivedOrdersView.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(o => o.ClientId == userId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.ArchivedDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.ArchivedDate <= toDate.Value);
            }

            query = descending
                ? query.OrderByDescending(o => o.ArchivedDate)
                : query.OrderBy(o => o.ArchivedDate);

            var archivedOrders = await query.ToListAsync();
            return Ok(archivedOrders);
        }

    }

}
