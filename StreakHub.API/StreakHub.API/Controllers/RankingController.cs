using Microsoft.AspNetCore.Mvc;
using StreakHub.API.Interfaces;

namespace StreakHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        // Cách Frontend gọi: GET /api/ranking/trending?timeFilter=week&clientToday=2026-06-23&topCount=10
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingShares(
            [FromQuery] string timeFilter,
            [FromQuery] DateOnly clientToday,
            [FromQuery] int topCount = 10)
        {
            // 1. Validate tham số đầu vào
            var validFilters = new[] { "today", "week", "all" };
            if (string.IsNullOrEmpty(timeFilter) || !validFilters.Contains(timeFilter.ToLower()))
            {
                return BadRequest(new
                {
                    Message = "Tham số timeFilter không hợp lệ. Vui lòng truyền 'today', 'week', hoặc 'all'."
                });
            }

            // 2. Chuyển cho Service xử lý
            var result = await _rankingService.GetTopSharesAsync(timeFilter, clientToday, topCount);

            // 3. Trả về Response
            return Ok(result);
        }
    }
}