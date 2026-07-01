using StreakHub.API.DTOs;

namespace StreakHub.API.Interfaces
{
    public interface IStarService
    {
        Task<StarDTO> ToggleStarAsync(int currentUserId, int shareId);
    }
}
