namespace StreakHub.API.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly TaskDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }


        public virtual User User { get; set; } = null!;
        public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
        public virtual ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
    }
}