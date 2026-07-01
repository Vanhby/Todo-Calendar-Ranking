
﻿using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.DTOs;
using StreakHub.API.Interfaces;
﻿using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<ShareResponseDTO> CreateShareAsync(ShareCreateDTO dto)
        {
            // Tự động generate ShareCode ngẫu nhiên (Lấy 8 ký tự từ mảng Guid và viết hoa)
            string randomShareCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            var share = new Share
            {
                UserId = dto.UserId,
                Title = dto.Title,
                TargetDate = dto.TargetDate, // Lưu thẳng DateOnly từ Frontend gửi lên (Luật 2)
                ShareCode = randomShareCode, // Dùng mã ngẫu nhiên vừa tạo
                CreatedAt = DateTime.UtcNow  // LUẬT 1: Cấm dùng DateTime.Now, ép dùng UtcNow chuẩn quốc tế
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
                TotalStars = 0 // Bài viết mới tạo mặc định sẽ có 0 sao
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