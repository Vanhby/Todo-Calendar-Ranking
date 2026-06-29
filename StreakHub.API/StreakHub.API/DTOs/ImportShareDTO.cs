namespace StreakHub.API.DTOs
{
    public class ImportShareDTO
    {
        public string ShareCode { get; set; } = string.Empty;
        public DateOnly TargetDate { get; set; }
        public List<int>? SelectedTodoIds { get; set; }
    }
}