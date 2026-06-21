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
    }
}