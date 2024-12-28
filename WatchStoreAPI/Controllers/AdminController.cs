using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchStoreAPI.Data;
using WatchStoreAPI.Models;

namespace WatchStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.User)
                .Select(e => new
                {
                    e.Id,
                    e.Position,
                    e.HireDate,
                    UserInfo = new
                    {
                        e.User.Name,
                        e.User.Email,
                        e.User.Phone
                    }
                })
                .ToListAsync();

            return Ok(employees);
        }
        [HttpPost("employees")]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Phone == request.Phone))
            {
                return BadRequest("Пользователь с данным email или телефоном уже существует.");
            }
            var employeeRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Employee");
            if (employeeRole == null)
            {
                return BadRequest("Роль 'Employee' не найдена. Обратитесь к системному администратору.");
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = passwordHash,
                RoleId = employeeRole.Id
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var newEmployee = new Employee
            {
                UserId = newUser.Id,
                Position = request.Position
            };
            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Новый сотрудник успешно добавлен!" });
        }

        [HttpDelete("employees/{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            var employee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
            {
                return NotFound("Сотрудник не найден.");
            }

            _context.Employees.Remove(employee);

            _context.Users.Remove(employee.User);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Сотрудник и его учетная запись успешно удалены." });
        }
    }

    public class AddEmployeeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}
