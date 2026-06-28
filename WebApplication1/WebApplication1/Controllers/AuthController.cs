using Microsoft.AspNetCore.Mvc;
using StreakHub.API.DataToObject;
using StreakHub.API.Service;
using System.Threading.Tasks; // Đảm bảo có thư viện này để dùng lớp Task

namespace StreakHub.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly Auth_Service _authService;

        public AuthController(Auth_Service authService)
        {
            _authService = authService;
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

            return Ok(new { Success = true, Message = statusMessage });
        }
    }
}