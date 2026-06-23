using StreakHub.API.DTOs;

namespace StreakHub.API.Interfaces
{
    public interface IRankingService
    {
        Task<List<RankingDTO>> GetTopSharesAsync(string timeFilter, DateOnly clientToday, int topCount = 10);
    }
}
