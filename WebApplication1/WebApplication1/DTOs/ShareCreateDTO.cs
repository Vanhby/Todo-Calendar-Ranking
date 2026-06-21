namespace StreakHub.API.DTOs
{
    public class ShareCreateDTO
    {
        public DateOnly TargetDate { get; set; }
        public class ShareResponseDTO
        {
            public string ShareCode { get; set; } = string.Empty;
            public DateOnly TargetDate { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
