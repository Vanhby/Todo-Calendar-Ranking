namespace StreakHub.API.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Thêm trường Color để hứng mã màu từ API (Endpoint 12)
        public string Color { get; set; } = "#8b949e";

        public virtual ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
        public virtual ICollection<ShareTag> ShareTags { get; set; } = new List<ShareTag>();
    }
}