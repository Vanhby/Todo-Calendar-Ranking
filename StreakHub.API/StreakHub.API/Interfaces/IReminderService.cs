using StreakHub.API.Models;

namespace StreakHub.API.Interfaces
{
    public interface IReminderService
    {
        Task<Reminder?> CreateReminderAsync(int taskId, DateTime notifyTime);
        Task<Reminder?> UpdateReminderAsync(int reminderId, DateTime notifyTime);
        Task<bool> DeleteReminderAsync(int reminderId);
        Task<List<Reminder>> GetUserRemindersAsync(int userId, bool includeFuture = false);
        Task<bool> MarkAsReadAsync(int reminderId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}
