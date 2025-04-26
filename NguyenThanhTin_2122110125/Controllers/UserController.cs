using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NguyenThanhTin_2122110125.Data;
using NguyenThanhTin_2122110125.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NguyenThanhTin_2122110125.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        private bool EmailExists(string email, int? excludeUserId = null)
        {
            return _context.Users.Any(u => u.Email == email && (!excludeUserId.HasValue || u.Id != excludeUserId));
        }

  
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this_is_a_super_secret_key_that_is_long_enough_123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("id", user.Id.ToString()),
                new Claim("name", user.Name),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "my_app",
                audience: "my_app",
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.Name,
                        Email = u.Email ?? "N/A",
                        Phone = u.Phone ?? "N/A",
                        Address = u.Address ?? "N/A",
                        Role = u.Role ?? "User",
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt,
                        DeletedAt = u.DeletedAt,
                        CreatedById = u.CreatedById ?? 0,
                        UpdatedById = u.UpdatedById ?? 0,
                        DeletedById = u.DeletedById ?? 0
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi khi lấy danh sách người dùng.",
                    error = ex.Message
                });
            }
        }








        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
            
                    return NotFound(new { message = "Không tìm thấy người dùng." });
                }

                return Ok(user);
            }
            catch (SqlException sqlEx) 
            {
                return StatusCode(500, new { message = "Lỗi kết nối cơ sở dữ liệu.", error = sqlEx.Message });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy người dùng.", error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            try
            {

                if (EmailExists(user.Email))
                    return BadRequest(new { message = "Email đã tồn tại!" });

                user.Role = user.Role == "1" ? "Admin" : "User";

                user.CreatedAt = DateTime.Now;
                user.IsActive = true;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
                {
                    message = "Tạo người dùng thành công",
                    user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo người dùng.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                if (id != user.Id)
                    return BadRequest(new { message = "Id không khớp!" });

                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    return NotFound(new { message = "Không tìm thấy người dùng." });

                if (EmailExists(user.Email, id))
                    return BadRequest(new { message = "Email đã tồn tại ở người dùng khác!" });

                existingUser.Name = user.Name ?? existingUser.Name;
                existingUser.Email = user.Email ?? existingUser.Email;
                existingUser.Phone = user.Phone ?? existingUser.Phone;
                existingUser.Address = user.Address ?? existingUser.Address;
                existingUser.Password = user.Password ?? existingUser.Password;
                existingUser.Password_Confirm = user.Password_Confirm ?? existingUser.Password_Confirm;
                existingUser.Role = user.Role == "1" ? "Admin" : "User";
                existingUser.IsActive = user.IsActive;
                existingUser.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật người dùng thành công", user = existingUser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật người dùng.", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { message = "Không tìm thấy người dùng." });

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã xoá người dùng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xoá người dùng.", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            try
            {
                if (EmailExists(newUser.Email))
                    return BadRequest(new { message = "Email đã tồn tại!" });

                if (newUser.Password != newUser.Password_Confirm)
                    return BadRequest(new { message = "Mật khẩu xác nhận không khớp!" });

                newUser.Role = newUser.Role == "1" ? "Admin" : "User";
                newUser.CreatedAt = DateTime.Now;
                newUser.IsActive = true;

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var token = GenerateJwtToken(newUser);

                return Ok(new
                {
                    message = "Đăng ký thành công!",
                    token,
                    user = newUser
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi đăng ký.", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
                if (user == null || user.Password != request.Password)
                    return Unauthorized(new { message = "Sai email hoặc mật khẩu!" });

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    message = "Đăng nhập thành công!",
                    token,
                    user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi đăng nhập.", error = ex.Message });
            }
        }
    }
}
