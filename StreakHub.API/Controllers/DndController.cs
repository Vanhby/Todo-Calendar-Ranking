using Microsoft.AspNetCore.Mvc;
using StreakHub.API.Interfaces;

namespace StreakHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DndController : ControllerBase
    {
        private readonly IDndService _dndService;

        public DndController(IDndService dndService)
        {
            _dndService = dndService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetDndSettings(int userId)
        {
            var user = await _dndService.GetDndSettingsAsync(userId);
            if (user == null) return NotFound("User not found");

            return Ok(new
            {
                Enabled = user.DndStart != null && user.DndEnd != null,
                Start = user.DndStart?.ToString(@"hh\:mm") ?? "22:00",
                End = user.DndEnd?.ToString(@"hh\:mm") ?? "07:00"
            });
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> UpdateDndSettings(int userId, [FromBody] DndUpdateRequest request)
        {
            if (!TimeSpan.TryParse(request.Start, out var start) || !TimeSpan.TryParse(request.End, out var end))
            {
                return BadRequest("Invalid time format. Use HH:mm");
            }

            var success = await _dndService.UpdateDndSettingsAsync(userId, request.Enabled, start, end);
            if (!success) return NotFound("User not found");

            return Ok(new { Message = "DND settings updated successfully" });
        }
    }

    public class DndUpdateRequest
    {
        public bool Enabled { get; set; }
        public required string Start { get; set; }
        public required string End { get; set; }
    }
}
