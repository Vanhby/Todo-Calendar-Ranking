using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.DTOs;
using StreakHub.API.Models;
using System;
using System.Threading.Tasks;
using static StreakHub.API.DTOs.ShareDTO;

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
            var share = await _context.Shares
                .FirstOrDefaultAsync(s => s.Id == id);

            if (share == null) return null;

            // Mapping sang DTO trước khi trả về, tránh lộ Entity Model
            return new ShareResponseDTO
            {
                Id = share.Id,
                UserId = share.UserId,
                TargetDate = share.TargetDate,
                ShareCode = share.ShareCode,
                CreatedAt = share.CreatedAt,
                Title = share.Title
            };
        }

        public async Task<ShareResponseDTO> CreateShareAsync(ShareCreateDTO dto)
        {
            var share = new Share
            {
                UserId = dto.UserId,
                Title = dto.Title,
                TargetDate = dto.TargetDate, // Lưu thẳng DateOnly từ Frontend gửi lên (Luật 2)
                ShareCode = dto.ShareCode,
                CreatedAt = DateTime.UtcNow // LUẬT 1: Cấm dùng DateTime.Now, ép dùng UtcNow chuẩn quốc tế
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
                Title = share.Title
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