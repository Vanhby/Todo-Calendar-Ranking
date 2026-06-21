namespace StreakHub.API.DTOs
{
    public class StreakResultDTO
    {
        public int CurrentStreak { get; set; } = 0;
        public int LongestStreak { get; set; } = 0;
        public int TotalTasksCompleted { get; set; } = 0;
    }
}