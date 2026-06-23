using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreakHub.API.Services;
using StreakHub.API.DTOs;

namespace StreakHub.API.Controllers
{
    [ApiController]
    [Route("api/rankings")]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        // GET /api/rankings/trending?timeframe=today&clientToday=2026-06-23&limit=10
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingShares([FromQuery] RankingRequestDTO request)
        {
            // Nếu Frontend quên gửi clientToday khi request today/week, hệ thống của C# 
            // có thể sẽ bắt lỗi ModelState tự động do DateOnly không hợp lệ.

            var result = await _rankingService.GetTrendingSharesAsync(request);
            return Ok(result);
        }
    }
}