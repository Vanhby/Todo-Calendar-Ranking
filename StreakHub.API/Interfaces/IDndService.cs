using StreakHub.API.Models;

namespace StreakHub.API.Interfaces
{
    public interface IDndService
    {
        Task<User?> GetDndSettingsAsync(int userId);
        Task<bool> UpdateDndSettingsAsync(int userId, bool enabled, TimeSpan startTime, TimeSpan endTime);
        Task<bool> IsUserInDndAsync(int userId);
        bool IsTimeInDnd(TimeSpan currentTime, TimeSpan start, TimeSpan end);
        Task<bool> AreNotificationsAllowedAsync(int userId);
        Task<bool> AreEmailsAllowedAsync(int userId);
        bool CheckDndTimeValid(string startTimeStr, string endTimeStr, out TimeSpan startTime, out TimeSpan endTime, out string errorMessage);
        bool CheckUserDndStatus(bool enabled, TimeSpan startTime, TimeSpan endTime, TimeSpan currentTime, out string statusMessage);
    }
}
