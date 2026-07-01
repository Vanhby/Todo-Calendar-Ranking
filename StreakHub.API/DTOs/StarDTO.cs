namespace StreakHub.API.DTOs
{
    public class StarDTO
    {
        public bool IsStarred { get; set; } // true: vừa thả sao, false: vừa hủy sao
        public string Message { get; set; } = string.Empty;
    }
}
