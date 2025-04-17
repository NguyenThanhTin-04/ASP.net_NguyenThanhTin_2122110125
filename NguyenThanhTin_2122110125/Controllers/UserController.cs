using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using NguyenThanhTin_2122110125.Data;
using NguyenThanhTin_2122110125.Model;
using NguyenThanhTin_2122110125.Services;
using BCrypt.Net;

namespace NguyenThanhTin_2122110125.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public UserController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
                return BadRequest("Email đã tồn tại!");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = hashedPassword,
                Address = model.Address,
                Description = model.Description
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user.Email);

            return Ok(new { message = "Đăng ký thành công", token, user.Name, user.Email });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login model)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                return Unauthorized("Sai email hoặc mật khẩu");

            var token = _jwtService.GenerateToken(user.Email);

            return Ok(new { message = "Đăng nhập thành công", token, user.Name, user.Email });
        }
    }
}
