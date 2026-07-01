namespace StreakHub.API.Models
{
    public class ShareTag
    {
        public int ShareId { get; set; }
        public int TagId { get; set; }

        public virtual Share Share { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}