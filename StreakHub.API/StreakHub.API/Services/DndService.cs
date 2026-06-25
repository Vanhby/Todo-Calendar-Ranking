using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.Interfaces;
using StreakHub.API.Models;

namespace StreakHub.API.Services
{
    public class DndService : IDndService
    {
        private readonly AppDbContext _context;

        public DndService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetDndSettingsAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> UpdateDndSettingsAsync(int userId, bool enabled, TimeSpan startTime, TimeSpan endTime)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return false;

            if (enabled)
            {
                user.DndStart = startTime;
                user.DndEnd = endTime;
            }
            else
            {
                user.DndStart = null;
                user.DndEnd = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUserInDndAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null || user.DndStart == null || user.DndEnd == null)
            {
                return false;
            }

            var currentTime = DateTime.Now.TimeOfDay;
            return IsTimeInDnd(currentTime, user.DndStart.Value, user.DndEnd.Value);
        }

        public bool IsTimeInDnd(TimeSpan currentTime, TimeSpan start, TimeSpan end)
        {
            if (start <= end)
            {
                return currentTime >= start && currentTime <= end;
            }
            else
            {
                return currentTime >= start || currentTime < end;
            }
        }

        public async Task<bool> AreNotificationsAllowedAsync(int userId)
        {
            return !await IsUserInDndAsync(userId);
        }

        public async Task<bool> AreEmailsAllowedAsync(int userId)
        {
            return !await IsUserInDndAsync(userId);
        }
    }
}
