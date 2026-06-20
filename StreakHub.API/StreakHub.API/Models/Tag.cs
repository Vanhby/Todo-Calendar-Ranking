namespace StreakHub.API.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
        public virtual ICollection<ShareTag> ShareTags { get; set; } = new List<ShareTag>();
    }
}