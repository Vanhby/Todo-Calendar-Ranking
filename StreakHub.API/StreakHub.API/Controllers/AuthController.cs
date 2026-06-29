using Microsoft.AspNetCore.Mvc;
using StreakHub.API.DataToObject;
using StreakHub.API.Service;
using StreakHub.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks; // Đảm bảo có thư viện này để dùng lớp Task

namespace StreakHub.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly Auth_Service _authService;
        private readonly AppDbContext _context;

        public AuthController(Auth_Service authService, AppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        /// <summary>
        /// Thêm từ khóa 'async' và chuyển đổi trả về thành 'Task<IActionResult>'
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Thêm từ khóa 'await' - Hệ thống sẽ tự động suy luận (Infer) được kiểu dữ liệu của tuple (isSuccess, statusMessage)
            var (isSuccess, statusMessage) = await _authService.Register(registerDto);

            if (!isSuccess)
            {
                return BadRequest(new { Success = false, Message = statusMessage });
            }

            return Ok(new { Success = true, Message = statusMessage });
        }

        /// <summary>
        /// Thêm từ khóa 'async' và chuyển đổi trả về thành 'Task<IActionResult>'
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Thêm từ khóa 'await' tại đây
            var (isSuccess, statusMessage) = await _authService.Login(loginDto);

            if (!isSuccess)
            {
                return BadRequest(new { Success = false, Message = statusMessage });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId); // Thích thì cứ để lại không sao
            }

            // SỬA DÒNG NÀY: Bổ sung UserId vào dữ liệu trả về
            return Ok(new { Success = true, Message = statusMessage, UserId = user?.UserId });
        }

        /// <summary>
        /// Lấy thông tin người dùng hiện tại từ session
        /// </summary>
        [HttpGet("me/{userId}")] // Đổi Route thành có nhận tham số
        public async Task<IActionResult> GetCurrentUser(int userId) // Nhận userId ở đây
        {
            // Xóa đoạn check Session cũ đi
            if (userId == 0)
            {
                return Unauthorized(new { Success = false, Message = "Chưa đăng nhập" });
            }

            var user = await _context.Users.Select(u => new
            {
                u.UserId,
                u.Username,
                u.Email,
                u.AvatarUrl,
                u.CreatedAt
            }).FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound(new { Success = false, Message = "Không tìm thấy người dùng" });
            }

            return Ok(new { Success = true, Data = user });
        }
    }
}