using Microsoft.AspNetCore.Mvc;
using StreakHub.API.DTOs;
using StreakHub.API.Services.Interfaces;

namespace StreakHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShareController : ControllerBase
    {
        private readonly IShareService _shareService;

        public ShareController(IShareService shareService)
        {
            _shareService = shareService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateShare([FromBody] ShareCreateDTO dto)
        {
            // Giả sử bạn đã có hàm lấy UserId từ Token (JWT)
            int currentUserId = GetUserIdFromToken();

            var result = await _shareService.CreateShareAsync(currentUserId, dto);

            return Ok(result); // Trả về Status 200 kèm DTO
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShare(int id)
        {
            try
            {
                // Lấy UserId của người đang đăng nhập từ thẻ bài JWT
                int currentUserId = GetUserIdFromToken();

                // Truyền sang Service xử lý
                var isDeleted = await _shareService.DeleteShareAsync(currentUserId, id);

                // Trả về DTO thông báo thành công (Tuân thủ Quy tắc Dữ liệu)
                return Ok(new { message = "Xóa lịch chia sẻ thành công!" });
            }
            catch (Exception ex)
            {
                // Trả về lỗi 400 nếu có bất kỳ lỗi logic nào xảy ra (Ví dụ: không chính chủ)
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
