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