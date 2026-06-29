using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreakHub.API.DTOs;
using StreakHub.API.Interfaces; 
using System.Security.Claims;

namespace StreakHub.API.Controllers
{
    [Route("api/todos")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService; 

        public TodoController(ITodoService todoService) 
        {
            _todoService = todoService;
        }

        private int GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        // 7: Tạo 1 task mới
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TodoCreateRequest request, [FromQuery] string clientToday)
        {
            var today = DateOnly.Parse(clientToday);
            var taskId = await _todoService.CreateSingleTaskAsync(GetCurrentUserId(), request, today);
            return Ok(new { status = "success", id = taskId });
        }

        // 8: Tạo task lặp lại
        [HttpPost("recurring")]
        public async Task<IActionResult> CreateRecurringTask([FromBody] TodoRecurringRequest request, [FromQuery] string clientToday)
        {
            var today = DateOnly.Parse(clientToday);
            var count = await _todoService.CreateRecurringTasksAsync(GetCurrentUserId(), request, today);
            return Ok(new { status = "success", count = count });
        }

        // 9: Cập nhật Task
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TodoUpdateRequest request)
        {
            await _todoService.UpdateTaskAsync(id, GetCurrentUserId(), request);
            return Ok(new { status = "success" });
        }

        // 10: Xóa Task
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _todoService.DeleteTaskAsync(id, GetCurrentUserId());
            return Ok(new { status = "success" });
        }

        // 11: Lấy Todo theo ngày

        [HttpGet("day")]
        public async Task<IActionResult> GetTasksByDay([FromQuery] DateOnly date, [FromQuery] int userId) // Thêm userId vào đây
        {
            // Lấy dữ liệu thật từ Database dựa trên userId vừa nhận được
            var tasks = await _todoService.GetTasksByDayAsync(userId, date);
            return Ok(tasks);
        }
    }
}