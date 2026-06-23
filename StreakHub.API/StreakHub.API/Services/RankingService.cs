using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.DTOs;
using StreakHub.API.Interfaces;

namespace StreakHub.API.Services
{
    public class RankingService : IRankingService
    {
        private readonly AppDbContext _context;

        public RankingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RankingDTO>> GetTopSharesAsync(string timeFilter, DateOnly clientToday, int topCount = 10)
        {
            // Chuyển đổi clientToday thành mốc thời gian để so sánh với CreatedAt của bảng Stars.
            DateTime filterDateStart = DateTime.MinValue; // Mặc định là từ trước đến nay (all)

            if (timeFilter.ToLower() == "today")
            {
                // Lấy mốc từ 00:00:00 của ngày hiện tại trên máy client
                filterDateStart = clientToday.ToDateTime(TimeOnly.MinValue);
            }
            else if (timeFilter.ToLower() == "week")
            {
                // Lấy mốc từ 7 ngày trước so với máy client
                filterDateStart = clientToday.AddDays(-7).ToDateTime(TimeOnly.MinValue);
            }

            var query = await _context.Shares
                .Include(s => s.User)
                .Include(s => s.ShareTags).ThenInclude(st => st.Tag)
                .Select(s => new RankingDTO
                {
                    ShareId = s.Id,
                    ShareCode = s.ShareCode,
                    AuthorName = s.User.Username,
                    // Đếm số lượng sao thỏa mãn điều kiện thời gian (CreatedAt)
                    TotalStars = s.Stars.Count(star => star.CreatedAt >= filterDateStart),
                    Tags = s.ShareTags.Select(st => st.Tag.Name).ToList()
                })
                .Where(x => x.TotalStars > 0) // Lọc bỏ những bài 0 sao cho gọn bảng xếp hạng
                .OrderByDescending(x => x.TotalStars) // Sắp xếp giảm dần theo số sao
                .Take(topCount) // Lấy ra Top N
                .ToListAsync();

            return query;
        }
    }
}
