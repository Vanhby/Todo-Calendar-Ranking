using System.Collections.Generic;
using System.Threading.Tasks;
using StreakHub.API.DTOs;

namespace StreakHub.API.Services
{
    public interface IRankingService
    {
        Task<IEnumerable<ShareResponseDTO>> GetTrendingSharesAsync(RankingRequestDTO request);
    }
}