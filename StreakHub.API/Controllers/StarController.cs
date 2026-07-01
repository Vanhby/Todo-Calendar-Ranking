using Microsoft.AspNetCore.Mvc;
using StreakHub.API.Interfaces;
using StreakHub.API.Services;

namespace StreakHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StarController : ControllerBase
    {
        private readonly IStarService _starService;

        public StarController(IStarService starService)
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
                //int currentUserId = GetUserIdFromToken();
                int currentUserId = 1;

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
    }
}
