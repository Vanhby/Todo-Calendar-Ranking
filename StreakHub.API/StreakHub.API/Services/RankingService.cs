using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.DTOs;

namespace StreakHub.API.Services
{
    public class RankingService : IRankingService
    {
        private readonly AppDbContext _context;

        public RankingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShareResponseDTO>> GetTrendingSharesAsync(RankingRequestDTO request)
        {
            var query = _context.Shares.AsQueryable();

            // LUẬT 2 & 3: Lọc dựa trên TargetDate và ClientToday từ Frontend
            if (!string.IsNullOrEmpty(request.Timeframe))
            {
                if (request.Timeframe.ToLower() == "today")
                {
                    query = query.Where(s => s.TargetDate == request.ClientToday);
                }
                else if (request.Timeframe.ToLower() == "week")
                {
                    var startOfWeek = request.ClientToday.AddDays(-7);
                    query = query.Where(s => s.TargetDate >= startOfWeek && s.TargetDate <= request.ClientToday);
                }
            }
            // Nếu Timeframe là "all" thì không Where, lấy toàn bộ

            var trendingShares = await query
                .OrderByDescending(s => s.Stars.Count) // Ranking dựa trên tổng số sao
                .Take(request.Limit)
                .Select(s => new ShareResponseDTO
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    TargetDate = s.TargetDate,
                    ShareCode = s.ShareCode,
                    CreatedAt = s.CreatedAt,
                    Title = s.Title,
                    TotalStars = s.Stars.Count // Tối ưu: EF Core tự dịch thành COUNT() trong SQL
                })
                .ToListAsync();

            return trendingShares;
        }
    }
}