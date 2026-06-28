namespace StreakHub.API.ShareTest
{
    public class ShareCodeModel
    {
        public int Id { get; set; }
        public string ShareCode { get; set; } = string.Empty;
        public string ExpectedResult { get; set; } = string.Empty;
        public string RealResult { get; set; } = string.Empty;
    }
}
