using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.DTOs;
using StreakHub.API.Interfaces;

namespace StreakHub.API.Services
{
    public class StreakService : IStreakService
    {
        private readonly AppDbContext _context;

        public StreakService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StreakResultDTO> GetUserStreakAsync(int userId)
        {
            try
            {
                var streakResult = await _context.Todos
                .Where(s => s.UserId == userId)
                .Select(s => new StreakResultDTO
                {
                    CurrentStreak = ,
                    LongestStreak = s.LongestStreak,
                    TotalTasksCompleted = 
                })
                .FirstOrDefaultAsync();
            }
            catch
            {
                throw new Exception("An error occurred while retrieving the user's streak.");
            }
        }

        public async Task<List<CalendarDayDTO>> GetMonthCalendarAsync(int userId, int year, int month)
        {
           
            throw new NotImplementedException();
        }
    }
}