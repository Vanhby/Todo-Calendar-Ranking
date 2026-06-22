using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
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

        public async Task<StreakResultDTO> GetUserStreakAsync(int userId, DateOnly clientToday)
        {
            try
            {
                var pastTodos = await _context.Todos
                    .Where(t => t.UserId == userId && t.TaskDate <= clientToday)
                    .ToListAsync();

                var totalCompleted = pastTodos.Count(t => t.IsCompleted);
                var groupedByDate = pastTodos
                    .GroupBy(t => t.TaskDate)
                    .OrderByDescending(g => g.Key)
                    .ToList();

                int currentStreak = 0;
                foreach (var group in groupedByDate)
                {
                    var date = group.Key;
                    var totalTasks = group.Count();
                    var completedTasks = group.Count(t => t.IsCompleted);
                    if (totalTasks > 0 && totalTasks == completedTasks)
                    {
                        currentStreak++;
                    }
                    else
                    {
                        if (date == clientToday)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return new StreakResultDTO
                {
                    CurrentStreak = currentStreak,
                    LongestStreak = 0,
                    TotalTasksCompleted = totalCompleted
                };
            }
            catch
            {
                throw new Exception("Lỗi khi tính toán chuỗi Streak của người dùng.");
            }
        }

        public async Task<List<CalendarDayDTO>> GetMonthCalendarAsync(int userId, int year, int month)
        {
            var startDate = new DateOnly(year, month, 1);
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var endDate = new DateOnly(year, month, daysInMonth);

            var rawTodos = await _context.Todos
                .Include(t => t.TodoTags)
                .ThenInclude(tt => tt.Tag) 
                .Where(t => t.UserId == userId && t.TaskDate >= startDate && t.TaskDate <= endDate)
                .ToListAsync();

            var response = new List<CalendarDayDTO>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDate = new DateOnly(year, month, day);
                var todosForDay = rawTodos.Where(t => t.TaskDate == currentDate).ToList();
                response.Add(new CalendarDayDTO
                {
                    Date = currentDate,
                    Tasks = todosForDay.Select(t => new TodoItemDTO
                    {
                        Id = t.Id,
                        Title = t.Title,
                        IsCompleted = t.IsCompleted,
                        Tags = t.TodoTags.Select(tt => tt.Tag.Name).ToList()
                    }).ToList()
                });
            }

            return response;
        }
    }
}