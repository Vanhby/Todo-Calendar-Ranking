using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreakHub.API.DTOs;
using StreakHub.API.Services;
using System.Security.Claims;
using static StreakHub.API.DTOs.TodoDTOs;

namespace StreakHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu JWT token
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;

        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // 5: Tạo 1 task mới 
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TodoCreateRequest request, [FromQuery] string clientToday)
        {
            var today = DateOnly.Parse(clientToday);
            var taskId = await _todoService.CreateSingleTaskAsync(GetCurrentUserId(), request, today);
            return Ok(new { status = "success", taskId = taskId });
        }

        // 7: Cập nhật Task (Check/Sửa) 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TodoUpdateRequest request)
        {
            await _todoService.UpdateTaskAsync(id, GetCurrentUserId(), request);
            return Ok(new { status = "success" });
        }

        // 8: Xóa Task 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _todoService.DeleteTaskAsync(id, GetCurrentUserId());
            return Ok(new { status = "success" });
        }

        // 9: Lấy Todo trong 1 ngày 
        [HttpGet("day")]
        public async Task<IActionResult> GetTasksByDay([FromQuery] string date)
        {
            var targetDate = DateOnly.Parse(date);
            var tasks = await _todoService.GetTasksByDayAsync(GetCurrentUserId(), targetDate);
            return Ok(tasks);
        }
    }
}