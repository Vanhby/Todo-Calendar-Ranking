namespace StreakHub.API.Models
{
    public class Star
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ShareId { get; set; }
        public DateTime CreatedAt { get; set; }


        public virtual User User { get; set; } = null!;
        public virtual Share Share { get; set; } = null!;
    }
}