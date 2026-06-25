using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.Interfaces;
using StreakHub.API.Models;

namespace StreakHub.API.Services
{
    public class ReminderService : IReminderService
    {
        private readonly AppDbContext _context;
        private readonly IDndService _dndService;
        
        private static readonly ConcurrentDictionary<int, bool> _readReminders = new();

        public ReminderService(AppDbContext context, IDndService dndService)
        {
            _context = context;
            _dndService = dndService;
        }

        public async Task<Reminder?> CreateReminderAsync(int taskId, DateTime notifyTime)
        {
            var todo = await _context.Todos.FindAsync(taskId);
            if (todo == null) return null;

            var existingReminders = await _context.Reminders.Where(r => r.TaskId == taskId).ToListAsync();
            if (existingReminders.Count > 0)
            {
                _context.Reminders.RemoveRange(existingReminders);
                foreach (var er in existingReminders)
                {
                    _readReminders.TryRemove(er.Id, out _);
                }
            }

            var reminder = new Reminder
            {
                TaskId = taskId,
                NotifyTime = DateTime.SpecifyKind(notifyTime, DateTimeKind.Utc),
                IsSent = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();
            return reminder;
        }

        public async Task<Reminder?> UpdateReminderAsync(int reminderId, DateTime notifyTime)
        {
            var reminder = await _context.Reminders
                .Include(r => r.Todo)
                .FirstOrDefaultAsync(r => r.Id == reminderId);
            if (reminder == null) return null;

            reminder.NotifyTime = DateTime.SpecifyKind(notifyTime, DateTimeKind.Utc);
            reminder.IsSent = false;

            _readReminders.TryRemove(reminderId, out _);

            await _context.SaveChangesAsync();
            return reminder;
        }

        public async Task<bool> DeleteReminderAsync(int reminderId)
        {
            var reminder = await _context.Reminders.FindAsync(reminderId);
            if (reminder == null) return false;

            _context.Reminders.Remove(reminder);
            _readReminders.TryRemove(reminderId, out _);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Reminder>> GetUserRemindersAsync(int userId, bool includeFuture = false)
        {
            if (!includeFuture && await _dndService.IsUserInDndAsync(userId))
            {
                return new List<Reminder>();
            }

            var query = _context.Reminders
                .Include(r => r.Todo)
                .Where(r => r.Todo.UserId == userId);

            if (!includeFuture)
            {
                var now = DateTime.UtcNow;
                query = query.Where(r => r.NotifyTime <= now);
            }

            return await query.OrderByDescending(r => r.NotifyTime).ToListAsync();
        }

        public async Task<bool> MarkAsReadAsync(int reminderId)
        {
            var reminder = await _context.Reminders.FindAsync(reminderId);
            if (reminder == null) return false;

            _readReminders[reminderId] = true;
            return true;
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            if (await _dndService.IsUserInDndAsync(userId))
            {
                return 0;
            }

            var now = DateTime.UtcNow;
            var reminderIds = await _context.Reminders
                .Where(r => r.Todo.UserId == userId && r.NotifyTime <= now)
                .Select(r => r.Id)
                .ToListAsync();

            int unreadCount = 0;
            foreach (var id in reminderIds)
            {
                if (!_readReminders.TryGetValue(id, out var isRead) || !isRead)
                {
                    unreadCount++;
                }
            }

            return unreadCount;
        }

        public static bool IsReminderRead(int reminderId)
        {
            return _readReminders.TryGetValue(reminderId, out var isRead) && isRead;
        }
    }
}
