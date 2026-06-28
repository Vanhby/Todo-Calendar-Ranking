using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StreakHub.API.DataToObject;
using Microsoft.EntityFrameworkCore;
using StreakHub.API.Models_Generated;

namespace StreakHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context; // Giả định DBContext của bạn tên DataContext
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            // 1. Kiểm tra xem Username hoặc Email đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
            {
                return BadRequest("Username hoặc Email đã tồn tại.");
            }

            // 2. Hash mật khẩu bằng BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Tạo User mới
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            // 1. Tìm user bằng Username hoặc Email
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == request.Username);

            if (user == null)
            {
                return BadRequest("Tài khoản hoặc mật khẩu không chính xác.");
            }

            // 2. Kiểm tra mật khẩu (So sánh Pass thô với PassHash trong DB)
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Tài khoản hoặc mật khẩu không chính xác.");
            }

            // 3. Đúng mật khẩu -> Tạo Token JWT trả về cho Client
            string token = CreateToken(user);

            return Ok(new { Token = token, Message = "Đăng nhập thành công!" });
        }

        // Hàm helper sinh chuỗi JWT Token
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1), // Token có hạn 1 ngày
                SigningCredentials = creds,
                Issuer = _configuration.GetSection("Jwt:Issuer").Value,
                Audience = _configuration.GetSection("Jwt:Audience").Value
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(token);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}