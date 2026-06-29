using System;

namespace StreakHub.API.DTOs
{
    // Giữ nguyên ShareCreateDTO
    public class ShareCreateDTO
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly TargetDate { get; set; }
        public string ShareCode { get; set; } = string.Empty;
    }

    // Cập nhật ShareResponseDTO
    public class ShareResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateOnly TargetDate { get; set; }
        public string ShareCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalStars { get; set; }
    }
}