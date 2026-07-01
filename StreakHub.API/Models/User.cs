namespace StreakHub.API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = "default.png";
        public string Email { get; set; } = string.Empty;
        public string? Code { get; set; }
        public TimeSpan? DndStart { get; set; }
        public TimeSpan? DndEnd { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
        public virtual ICollection<Share> Shares { get; set; } = new List<Share>();
        public virtual ICollection<Star> Stars { get; set; } = new List<Star>();
    }
}