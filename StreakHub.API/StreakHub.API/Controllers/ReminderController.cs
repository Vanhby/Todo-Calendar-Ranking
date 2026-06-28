using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreakHub.API.DTOs;
using StreakHub.API.Interfaces;
using StreakHub.API.Services;
using StreakHub.API.Data;

namespace StreakHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _reminderService;
        private readonly AppDbContext _context;

        public ReminderController(IReminderService reminderService, AppDbContext context)
        {
            _reminderService = reminderService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReminder([FromBody] CreateReminderRequest request)
        {
            var reminder = await _reminderService.CreateReminderAsync(request.TaskId, request.NotifyTime);
            if (reminder == null) return BadRequest("Task not found or failed to create reminder");

            return Ok(new ReminderDto
            {
                Id = reminder.Id,
                TaskId = reminder.TaskId,
                Title = reminder.Todo?.Title ?? "Task",
                NotifyTime = reminder.NotifyTime,
                IsSent = reminder.IsSent,
                IsRead = false,
                CreatedAt = reminder.CreatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(int id, [FromBody] UpdateReminderRequest request)
        {
            var reminder = await _reminderService.UpdateReminderAsync(id, request.NotifyTime);
            if (reminder == null) return NotFound("Reminder not found");

            return Ok(new ReminderDto
            {
                Id = reminder.Id,
                TaskId = reminder.TaskId,
                Title = reminder.Todo?.Title ?? "Task",
                NotifyTime = reminder.NotifyTime,
                IsSent = reminder.IsSent,
                IsRead = false,
                CreatedAt = reminder.CreatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            var success = await _reminderService.DeleteReminderAsync(id);
            if (!success) return NotFound("Reminder not found");

            return Ok(new { Message = "Reminder deleted successfully" });
        }

        [HttpDelete("task/{taskId}")]
        public async Task<IActionResult> DeleteReminderByTask(int taskId)
        {
            var reminder = await _context.Reminders.FirstOrDefaultAsync(r => r.TaskId == taskId);
            if (reminder == null) return NotFound("Reminder not found");

            var success = await _reminderService.DeleteReminderAsync(reminder.Id);
            if (!success) return NotFound("Reminder could not be deleted");

            return Ok(new { Message = "Reminder deleted successfully" });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserReminders(int userId, [FromQuery] bool includeFuture = false)
        {
            var reminders = await _reminderService.GetUserRemindersAsync(userId, includeFuture);
            var dtos = reminders.Select(r => new ReminderDto
            {
                Id = r.Id,
                TaskId = r.TaskId,
                Title = r.Todo?.Title ?? "Task",
                NotifyTime = r.NotifyTime,
                IsSent = r.IsSent,
                IsRead = ReminderService.IsReminderRead(r.Id),
                CreatedAt = r.CreatedAt
            }).ToList();

            return Ok(dtos);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var success = await _reminderService.MarkAsReadAsync(id);
            if (!success) return NotFound("Reminder not found");

            return Ok(new { Message = "Reminder marked as read" });
        }

        [HttpPost("user/{userId}/read-all")]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            var reminders = await _reminderService.GetUserRemindersAsync(userId);
            foreach (var r in reminders)
            {
                await _reminderService.MarkAsReadAsync(r.Id);
            }
            return Ok(new { Message = "All reminders marked as read" });
        }

        [HttpGet("user/{userId}/unread-count")]
        public async Task<IActionResult> GetUnreadCount(int userId)
        {
            var count = await _reminderService.GetUnreadCountAsync(userId);
            return Ok(new { Count = count });
        }
    }

    public class CreateReminderRequest
    {
        public int TaskId { get; set; }
        public DateTime NotifyTime { get; set; }
    }

    public class UpdateReminderRequest
    {
        public DateTime NotifyTime { get; set; }
    }
}
