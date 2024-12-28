using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WatchStoreAPI.Data;

namespace WatchStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeClientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeClientsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _context.Set<ClientUser>().ToListAsync();
            return Ok(clients);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchClients([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await GetAllClients();
            }
            var clients = await _context.Set<ClientUser>()
                .Where(c => EF.Functions.ILike(c.ClientName, $"%{name}%"))
                .ToListAsync();

            return Ok(clients);
        }

        public class ClientUser
        {
            public int ClientId { get; set; }
            public string ClientName { get; set; } = string.Empty;
            public string ClientEmail { get; set; } = string.Empty;
            public string ClientPhone { get; set; } = string.Empty;
        }
    }
}
