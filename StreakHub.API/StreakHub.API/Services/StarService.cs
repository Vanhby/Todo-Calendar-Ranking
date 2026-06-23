using StreakHub.API.Data;
using StreakHub.API.DTOs;
using StreakHub.API.Interfaces;
using StreakHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace StreakHub.API.Services
{
    public class StarService : IStarService
    {
        private readonly AppDbContext _context;

        public StarService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<StarDTO> ToggleStarAsync(int currentUserId, int shareId)
        {
            // 1. Kiểm tra bài Share có tồn tại thực tế trong DB không
            var share = await _context.Shares.FindAsync(shareId);
            if (share == null)
            {
                throw new Exception("Không tìm thấy lịch trình chia sẻ này.");
            }

            // 2. NGHIỆP VỤ: Tuyệt đối không cho phép User thả sao cho chính bài Share của mình
            if (share.UserId == currentUserId)
            {
                throw new Exception("Bạn không thể tự thả sao cho lịch trình của chính mình!");
            }

            // 3. Kiểm tra xem cặp (UserId, ShareId) đã từng tồn tại trong bảng Stars chưa
            var existingStar = await _context.Stars
                .FirstOrDefaultAsync(x => x.UserId == currentUserId && x.ShareId == shareId);

            bool isStarred;
            string message;

            if (existingStar != null)
            {
                // 👉 ĐÃ TỒN TẠI: Người này muốn HỦY SAO -> Tiến hành xóa dòng khỏi DB
                _context.Stars.Remove(existingStar);
                isStarred = false;
                message = "Đã hủy đánh giá sao thành công.";
            }
            else
            {
                // 👉 CHƯA TỒN TẠI: Người này muốn THẢ SAO -> Tiến hành thêm mới dòng vào DB
                var newStar = new Star
                {
                    UserId = currentUserId,
                    ShareId = shareId,
                    CreatedAt = DateTime.UtcNow // TUÂN THỦ LUẬT 1: Luôn luôn dùng UtcNow để lưu giờ chuẩn
                };

                _context.Stars.Add(newStar);
                isStarred = true;
                message = "Đã thả sao thành công.";
            }

            // 4. Lưu toàn bộ thay đổi xuống database dưới dạng một Transaction an toàn
            await _context.SaveChangesAsync();

            // 5. Đếm lại tổng số lượng sao hiện tại của bài share này để Frontend đồng bộ realtime
            int totalStars = await _context.Stars.CountAsync(x => x.ShareId == shareId);

            // 6. Đóng gói kết quả vào DTO để trả về lớp trên
            return new StarDTO
            {
                IsStarred = isStarred,
                TotalStars = totalStars,
                Message = message
            };
        }
    }
}
