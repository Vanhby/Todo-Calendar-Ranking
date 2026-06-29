using Microsoft.AspNetCore.Mvc;
using StreakHub.API.DTOs;
using StreakHub.API.Services;

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

        // GET /api/shares/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShareById(int id)
        {
            var result = await _shareService.GetShareByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy bài viết với Id = {id}" });
            }
            return Ok(result);
        }

        // POST /api/shares
        [HttpPost]
        public async Task<IActionResult> CreateShare([FromBody] ShareCreateDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Dữ liệu yêu cầu không hợp lệ." });
            }

            var result = await _shareService.CreateShareAsync(dto);
            return Ok(result);
        }

        // DELETE /api/shares/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShare(int id)
        {
            var isDeleted = await _shareService.DeleteShareAsync(id);
            if (!isDeleted)
            {
                return BadRequest(new { message = "Xóa thất bại. Bài viết không tồn tại hoặc đã bị xóa trước đó." });
            }
            return Ok(new { message = "Xóa bài viết thành công." });
        }
    }
}