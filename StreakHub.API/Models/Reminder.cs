namespace StreakHub.API.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public DateTime NotifyTime { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; }


        public virtual Todo Todo { get; set; } = null!;
    }
}