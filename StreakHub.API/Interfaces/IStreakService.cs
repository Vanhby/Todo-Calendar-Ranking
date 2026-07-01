using StreakHub.API.DTOs;

namespace StreakHub.API.Interfaces
{
    public interface IStreakService
    {
        Task<StreakResultDTO> GetUserStreakAsync(int userId, DateOnly clientToday);
        Task<List<CalendarDayDTO>> GetMonthCalendarAsync(int userId, int year, int month);
    }
}