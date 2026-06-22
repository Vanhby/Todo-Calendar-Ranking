using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.DTOs;
using StreakHub.API.Interfaces;
using StreakHub.API.Models;

namespace StreakHub.API.Services
{
    public class ShareService : IShareService
    {
        private readonly AppDbContext _context;

        public ShareService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ImportSharedListAsync(int currentUserId, ImportShareDTO dto)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

            if (dto.TargetDate < today)
            {
                throw new Exception("Không được phép import lịch trình vào các ngày trong quá khứ!");
            }

            var shareRecord = await _context.Shares
                .FirstOrDefaultAsync(s => s.ShareCode == dto.ShareCode);

            if (shareRecord == null) throw new Exception("Mã chia sẻ không tồn tại hoặc đã hết hạn!");

            var originalTodosQuery = _context.Todos
                .Include(t => t.TodoTags)
                .Where(t => t.UserId == shareRecord.UserId && t.TaskDate == shareRecord.TargetDate);

            if (dto.SelectedTodoIds != null && dto.SelectedTodoIds.Any())
            {
                originalTodosQuery = originalTodosQuery.Where(t => dto.SelectedTodoIds.Contains(t.Id));
            }

            var todosToCopy = await originalTodosQuery.ToListAsync();

            foreach (var oldTodo in todosToCopy)
            {
                var newTodo = new Todo
                {
                    UserId = currentUserId,
                    Title = oldTodo.Title,
                    TaskDate = dto.TargetDate,
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    TodoTags = new List<TodoTag>()
                };

                foreach (var oldTodoTag in oldTodo.TodoTags)
                {
                    newTodo.TodoTags.Add(new TodoTag { TagId = oldTodoTag.TagId });
                }

                _context.Todos.Add(newTodo);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}