using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WatchStoreAPI.Data;
using WatchStoreAPI.Models;

namespace WatchStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #region Получить всех пользователей
        [HttpGet]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Phone,
                    Role = u.Role != null ? u.Role.Name : "No Role"
                })
                .ToListAsync();

            return Ok(users);
        }
        #endregion

        #region Авторизация пользователя
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null || !VerifyPassword(login.Password, user.PasswordHash))
            {
                return Unauthorized("Неверные email или пароль");
            }

            // Создаём JWT-токен
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role?.Name ?? "Unknown"),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"])),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new AuthResponse
            {
                Token = tokenString,
                UserName = user.Name,
                Role = user.Role?.Name ?? "Unknown",
                UserId = user.Id
            };


            return Ok(response);
        }

        #endregion

        #region Вспомогательная логика
        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        #endregion

        #region Регистрация пользователя
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegister register)
        {
            if (await _context.Users.AnyAsync(u => u.Email == register.Email || u.Phone == register.Phone))
            {
                return BadRequest("Пользователь с данным email или телефоном уже существует.");
            }

            var clientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Client");
            if (clientRole == null)
            {
                return BadRequest("Роль 'Client' не найдена. Обратитесь к администратору.");
            }

            var newUser = new User
            {
                Name = register.Name,
                Email = register.Email,
                Phone = register.Phone,
                PasswordHash = HashPassword(register.Password),
                RoleId = clientRole.Id
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Пользователь {register.Name} успешно зарегистрирован!" });
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        #endregion
    }
}
