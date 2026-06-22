using StreakHub.API.DTOs;
using static StreakHub.API.DTOs.ShareCalendarDTO;

namespace StreakHub.API.Interfaces
{
    public interface IShareService
    {
        Task<ShareResponseDTO> CreateShareAsync(int userId, ShareCalendarDTO dto);
        Task<bool> DeleteShareAsync(int userId, int shareId);
    }
}
