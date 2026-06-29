using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.DTOs;
using StreakHub.API.Interfaces;
using StreakHub.API.Models;

namespace StreakHub.API.Services
{
    public class TodoService : ITodoService
    {
        private readonly AppDbContext _context;

        public TodoService(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint 7: Tạo task mới (Lưu kèm mảng TagIds)
        public async Task<int> CreateSingleTaskAsync(int userId, TodoCreateRequest request, DateOnly clientToday)
        {
            var taskDate = DateOnly.Parse(request.TaskDate);
            if (taskDate < clientToday)
            {
                throw new Exception("Không cho phép thêm Todo với TaskDate nằm trong quá khứ");
            }

            var newTodo = new Todo
            {
                UserId = userId,
                Title = request.Title,
                TaskDate = taskDate,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            // Lưu danh sách Tag vào bảng trung gian
            if (request.TagIds != null && request.TagIds.Any())
            {
                foreach (var tagId in request.TagIds)
                {
                    newTodo.TodoTags.Add(new TodoTag { TagId = tagId });
                }
            }

            _context.Todos.Add(newTodo);
            await _context.SaveChangesAsync();
            return newTodo.Id;
        }

        // 8: Tạo task lặp lại
        public async Task<int> CreateRecurringTasksAsync(int userId, TodoRecurringRequest request, DateOnly clientToday)
        {
            var newTodos = new List<Todo>();
            var currentDate = clientToday;
            int generatedCount = 0;

            var parts = request.Pattern.Split('_');
            if (parts.Length == 2 && parts[0] == "WEEKLY")
            {
                DayOfWeek targetDay = parts[1] switch
                {
                    "MON" => DayOfWeek.Monday,
                    "TUE" => DayOfWeek.Tuesday,
                    "WED" => DayOfWeek.Wednesday,
                    "THU" => DayOfWeek.Thursday,
                    "FRI" => DayOfWeek.Friday,
                    "SAT" => DayOfWeek.Saturday,
                    "SUN" => DayOfWeek.Sunday,
                    _ => throw new Exception("Pattern không hợp lệ")
                };

                while (currentDate.DayOfWeek != targetDay)
                {
                    currentDate = currentDate.AddDays(1);
                }

                for (int i = 0; i < 12; i++)
                {
                    newTodos.Add(new Todo
                    {
                        UserId = userId,
                        Title = request.Title,
                        TaskDate = currentDate.AddDays(i * 7),
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow
                    });
                    generatedCount++;
                }

                _context.Todos.AddRange(newTodos);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Chỉ hỗ trợ chu kỳ WEEKLY_XXX");
            }

            return generatedCount;
        }

        // 9: Cập nhật Task
        public async Task UpdateTaskAsync(int todoId, int userId, TodoUpdateRequest request)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);
            if (todo == null) throw new Exception("Không tìm thấy task hoặc không có quyền thao tác");

            todo.Title = request.Title;
            todo.IsCompleted = request.IsCompleted;
            await _context.SaveChangesAsync();
        }

        // 10: Xóa Task
        public async Task DeleteTaskAsync(int todoId, int userId)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);
            if (todo == null) throw new Exception("Không tìm thấy task hoặc không có quyền thao tác");

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }

        // 11: Lấy Todo theo ngày (Kéo theo Tags)
        public async Task<List<TodoResponse>> GetTasksByDayAsync(int userId, DateOnly date)
        {
            return await _context.Todos
                .Include(t => t.TodoTags)
                    .ThenInclude(tt => tt.Tag) // Kéo dữ liệu từ bảng Tag thật
                .Where(t => t.UserId == userId && t.TaskDate == date)
                .Select(t => new TodoResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    Tags = t.TodoTags.Select(tt => new TagDTO
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name
                    }).ToList()
                }).ToListAsync();
        }

        //12: Lấy DS Nhãn(Tags)
        public async Task<List<TagDTO>> GetAllTagsAsync()
        {
            return await _context.Tags
                .Select(t => new TagDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    Color = t.Color
                }).ToListAsync();
        }
    }
}