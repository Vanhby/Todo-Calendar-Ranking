namespace StreakHub.API.DTOs
{
    public class ShareDTO
    {
        public class ShareCreateDTO
        {
            public int UserId { get; set; }
            public string Title { get; set; } = string.Empty;

            // LUẬT 2: Frontend tự quyết định ngày và đẩy chuỗi lên, Backend dùng DateOnly hứng thẳng
            public DateOnly TargetDate { get; set; }
            public string ShareCode { get; set; } = string.Empty;
        }

        public class ShareResponseDTO
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public DateOnly TargetDate { get; set; }
            public string ShareCode { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public string Title { get; set; } = string.Empty;
        }
    }
}
