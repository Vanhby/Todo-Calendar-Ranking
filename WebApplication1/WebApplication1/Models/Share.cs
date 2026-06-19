namespace StreakHub.API.Models
{
    public class Share
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateOnly TargetDate { get; set; }
        public string ShareCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }


        public virtual User User { get; set; } = null!;
        public virtual ICollection<Star> Stars { get; set; } = new List<Star>();
    }
}