namespace StreakHub.API.DTOs
{
    public class ReminderDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime NotifyTime { get; set; }
        public bool IsSent { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
