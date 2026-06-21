using StreakHub.API.Data;
using StreakHub.API.DTOs;
using StreakHub.API.Models;
using StreakHub.API.Services.Interfaces;
using static StreakHub.API.DTOs.ShareCreateDTO;

namespace StreakHub.API.Services.Implementations
{
    public class ShareService : IShareService
    {
        private readonly AppDbContext _context;

        public ShareService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShareResponseDTO> CreateShareAsync(int userId, ShareCreateDTO dto)
        {
            // 1. Sinh mã ShareCode ngẫu nhiên (ví dụ: chuỗi 8 ký tự)
            string generatedCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            // 2. Tạo Entity để lưu xuống DB
            var newShare = new Share
            {
                UserId = userId,
                TargetDate = dto.TargetDate, // Lấy từ DTO do Frontend truyền lên
                ShareCode = generatedCode,
                CreatedAt = DateTime.UtcNow  // TUYỆT ĐỐI tuân thủ Luật 1: Dùng UtcNow
            };

            _context.Shares.Add(newShare);
            await _context.SaveChangesAsync();

            // 3. Đóng gói Entity thành DTO để trả về
            return new ShareResponseDTO
            {
                ShareCode = newShare.ShareCode,
                TargetDate = newShare.TargetDate,
                CreatedAt = newShare.CreatedAt
            };
        }

        public async Task<bool> DeleteShareAsync(int userId, int shareId)
        {
            // 1. Tìm bản ghi Share trong DB dựa vào Id
            var share = await _context.Shares.FindAsync(shareId);

            // 2. Nếu không tồn tại thì báo lỗi
            if (share == null)
            {
                throw new Exception("Không tìm thấy lịch trình chia sẻ này.");
            }

            // 3. BẢO MẬT: Kiểm tra xem người đang gọi API có phải là chủ sở hữu không
            if (share.UserId != userId)
            {
                throw new Exception("Bạn không có quyền xóa lịch trình chia sẻ của người khác!");
            }

            // 4. Xóa khỏi Database
            _context.Shares.Remove(share);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
