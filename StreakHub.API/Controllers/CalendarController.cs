using Microsoft.AspNetCore.Mvc;
using StreakHub.API.Interfaces;
using StreakHub.API.DTOs;
using System.ComponentModel.DataAnnotations;

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

        [HttpGet("month/{userId}")]
        public async Task<IActionResult> GetMonthCalendar(
            [FromRoute] int userId,
            [FromQuery, Required] int year,
            [FromQuery, Required] int month)
        {
            try
            {
                if (month < 1 || month > 12)
                {
                    return BadRequest("Tháng không hợp lệ (phải từ 1 đến 12).");
                }
                var result = await _streakService.GetMonthCalendarAsync(userId, year, month);
                return Ok(new { days = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("streak/{userId}")]
        public async Task<IActionResult> GetUserStreak([FromRoute] int userId)
        {
            try
            {
                var clientToday = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

                var result = await _streakService.GetUserStreakAsync(userId, clientToday);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
    }
}