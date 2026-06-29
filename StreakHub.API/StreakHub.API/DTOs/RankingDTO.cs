using System;

namespace StreakHub.API.DTOs
{
    public class RankingRequestDTO
    {
        // Nhận 3 giá trị: "today", "week", "all"
        public string Timeframe { get; set; } = "all";

        // LUẬT 3: Ép Frontend truyền "Hôm nay" của họ lên để lọc chính xác
        public DateOnly ClientToday { get; set; }

        // Số lượng top trending muốn lấy (mặc định Top 10)
        public int Limit { get; set; } = 10;
    }
}