using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.DTOs;
using StreakHub.API.Models;
using static StreakHub.API.DTOs.TodoDTOs;

namespace StreakHub.API.Services
{
    public class TodoService
    {
        private readonly AppDbContext _context;

        public TodoService(AppDbContext context)
        {
            _context = context;
        }

        //5: Tạo 1 task mới 
        public async Task<int> CreateSingleTaskAsync(int userId, TodoCreateRequest request, DateOnly clientToday)
        {
            var taskDate = DateOnly.Parse(request.Date);
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
                CreatedAt = DateTime.UtcNow // Luôn dùng UtcNow
            };

            _context.Todos.Add(newTodo);
            await _context.SaveChangesAsync();
            return newTodo.Id;
        }
        // 6 Tạo task lặp lại (Ví dụ gen ra 12 task cho 12 tuần)
        public async Task<int> CreateRecurringTasksAsync(int userId, TodoRecurringRequest request, DateOnly clientToday)
        {
            var newTodos = new List<Todo>();
            var currentDate = clientToday;
            int generatedCount = 0;

            // Tách chuỗi Pattern (VD: "WEEKLY_TUE" -> "WEEKLY" và "TUE")
            var parts = request.Pattern.Split('_');
            if (parts.Length == 2 && parts[0] == "WEEKLY")
            {
                // Xác định thứ trong tuần cần tạo
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

                // Lặp ngày hiện tại tiến lên phía trước cho đến khi gặp đúng Thứ đó
                while (currentDate.DayOfWeek != targetDay)
                {
                    currentDate = currentDate.AddDays(1);
                }

                // Tạo sẵn 12 task cho 12 tuần liên tiếp (Tương đương 3 tháng)
                for (int i = 0; i < 12; i++)
                {
                    newTodos.Add(new Todo
                    {
                        UserId = userId,
                        Title = request.Title,
                        TaskDate = currentDate.AddDays(i * 7), // Cộng thêm 7 ngày mỗi vòng lặp
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
                throw new Exception("Hệ thống hiện tại chỉ hỗ trợ chu kỳ WEEKLY_XXX");
            }

            return generatedCount;
        }
        //  7: Cập nhật Task 
        public async Task UpdateTaskAsync(int todoId, int userId, TodoUpdateRequest request)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);
            if (todo == null) throw new Exception("Không tìm thấy task hoặc không có quyền thao tác");

            todo.Title = request.Title;
            todo.IsCompleted = request.IsCompleted;
            await _context.SaveChangesAsync();
        }

        //8: Xóa Task 
        public async Task DeleteTaskAsync(int todoId, int userId)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);
            if (todo == null) throw new Exception("Không tìm thấy task hoặc không có quyền thao tác");

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }

        //9: Lấy Todo trong 1 ngày 
        public async Task<List<TodoResponse>> GetTasksByDayAsync(int userId, DateOnly date)
        {
            return await _context.Todos
                .Where(t => t.UserId == userId && t.TaskDate == date)
                .Select(t => new TodoResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted
                }).ToListAsync();
        }
        

        // [Endpoint 10] Lấy Todo cả tháng (Gom nhóm theo ngày)
        public async Task<Dictionary<string, List<TodoResponse>>> GetTasksByMonthAsync(int userId, int year, int month)
        {
            // Lấy toàn bộ task của user trong tháng đó
            var tasks = await _context.Todos
                .Where(t => t.UserId == userId && t.TaskDate.Year == year && t.TaskDate.Month == month)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.TaskDate,
                    t.IsCompleted
                })
                .ToListAsync();

            // Dùng LINQ để gom nhóm (Group By) trên RAM theo chuẩn chuỗi "yyyy-MM-dd"
            var groupedTasks = tasks
                .GroupBy(t => t.TaskDate.ToString("yyyy-MM-dd"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(t => new TodoResponse
                    {
                        Id = t.Id,
                        Title = t.Title,
                        IsCompleted = t.IsCompleted
                    }).ToList()
                );

            return groupedTasks;
        }
    }
}