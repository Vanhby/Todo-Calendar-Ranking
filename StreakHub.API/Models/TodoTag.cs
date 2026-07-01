namespace StreakHub.API.Models
{
    public class TodoTag
    {
        public int TodoId { get; set; }
        public int TagId { get; set; }

        public virtual Todo Todo { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}