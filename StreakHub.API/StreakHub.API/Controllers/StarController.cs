using Microsoft.AspNetCore.Mvc;
using StreakHub.API.Interfaces;
using StreakHub.API.Services;

namespace StreakHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RankingController : ControllerBase
    {
        private readonly IStarService _starService;

        public RankingController(IStarService starService)
        {
            _starService = starService;
        }

        // Endpoint Thả/Hủy sao: POST /api/ranking/toggle-star/12
        [HttpPost("toggle-star/{shareId}")]
        public async Task<IActionResult> ToggleStar(int shareId)
        {
            try
            {
                // Hàm giả định trích xuất thông tin UserId từ chuỗi Token JWT khi đăng nhập
                int currentUserId = GetUserIdFromToken();

                // Gọi lớp dịch vụ tính toán xử lý logic
                var result = await _starService.ToggleStarAsync(currentUserId, shareId);

                // Trả về Status 200 OK kèm dữ liệu DTO sạch sẽ
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Trả về Status 400 Bad Request kèm thông báo lỗi logic (VD: Tự thả sao chính mình)
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetUserIdFromToken()
        {
            // Logic đọc claim "NameIdentifier" hoặc "id" trong JWT token để trả về số INT nguyên bản
            // (Bạn áp dụng cách viết hàm bổ trợ mà team bạn đã thống nhất ở các Controller trước)
            return 1;
        }
    }
}
