using Microsoft.AspNetCore.Mvc;
using StreakHub.API.Interfaces;

namespace StreakHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IStreakService _streakService;

        public CalendarController(IStreakService streakService)
        {
            _streakService = streakService;
        }

        [HttpGet("streak/{userId}")]
        public async Task<IActionResult> GetStreak(int userId, [FromQuery] DateOnly clientToday)
        {
            var result = await _streakService.GetUserStreakAsync(userId, clientToday);
            return Ok(result);
        }

        [HttpGet("month/{userId}")]
        public async Task<IActionResult> GetMonthCalendar(int userId, [FromQuery] int year, [FromQuery] int month)
        {
            var result = await _streakService.GetMonthCalendarAsync(userId, year, month);
            return Ok(result);
        }
    }
}