using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.DTOs;
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

        public ShareService()
        {
        }

        public async Task<ShareResponseDTO?> GetShareByIdAsync(int id)
        {
            // Dùng .Select để tối ưu. EF Core sẽ tự dịch s.Stars.Count thành câu lệnh COUNT() trong SQL.
            var shareDTO = await _context.Shares
                .Where(s => s.Id == id)
                .Select(s => new ShareResponseDTO
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    TargetDate = s.TargetDate,
                    ShareCode = s.ShareCode,
                    CreatedAt = s.CreatedAt,
                    Title = s.Title,
                    TotalStars = s.Stars.Count // Đếm tổng số sao dựa vào ICollection<Star> của Model
                })
                .FirstOrDefaultAsync();

            return shareDTO;
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

        public bool ValidateShareCode(String shareCode)
        {
            if (shareCode == "")
            {
                return false;
            }
            else if (shareCode.Length < 4 || shareCode.Length > 16)
            {
                return false;
            }
            else if (!shareCode.All(char.IsLetterOrDigit))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<ShareResponseDTO> CreateShareAsync(ShareCreateDTO dto)
        {
            if(!ValidateShareCode(dto.ShareCode))
            {
                throw new Exception("Mã chia sẻ không hợp lệ!");
            }

            var share = new Share
            {
                UserId = dto.UserId,
                Title = dto.Title,
                TargetDate = dto.TargetDate,
                ShareCode = dto.ShareCode,
                CreatedAt = DateTime.UtcNow
            };

            _context.Shares.Add(share);
            await _context.SaveChangesAsync();

            return new ShareResponseDTO
            {
                Id = share.Id,
                UserId = share.UserId,
                TargetDate = share.TargetDate,
                ShareCode = share.ShareCode,
                CreatedAt = share.CreatedAt,
                Title = share.Title,
                TotalStars = 0
            };
        }

        public async Task<bool> DeleteShareAsync(int id)
        {
            var share = await _context.Shares.FindAsync(id);
            if (share == null) return false;

            _context.Shares.Remove(share);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}