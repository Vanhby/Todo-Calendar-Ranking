namespace StreakHub.API.DTOs
{
    public class StarDTO
    {
        public bool IsStarred { get; set; } // true: vừa thả sao, false: vừa hủy sao
        public int TotalStars { get; set; } // Tổng số sao hiện tại của bài share đó
        public string Message { get; set; } = string.Empty;
    }
}
