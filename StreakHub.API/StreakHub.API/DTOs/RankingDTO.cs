namespace StreakHub.API.DTOs
{
    public class RankingDTO
    {
        public int ShareId { get; set; }
        public string ShareCode { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int TotalStars { get; set; }

        // Trả về danh sách chuỗi tên nhãn (Tag) cho nhẹ dữ liệu
        public List<string> Tags { get; set; } = new List<string>();
    }
}
