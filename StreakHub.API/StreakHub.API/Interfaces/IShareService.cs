using StreakHub.API.DTOs;

namespace StreakHub.API.Interfaces
{
    public interface IShareService
    {
        Task<bool> ImportSharedListAsync(int currentUserId, ImportShareDTO dto);
    }
}