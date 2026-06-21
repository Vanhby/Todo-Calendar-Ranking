using StreakHub.API.DTOs;
using static StreakHub.API.DTOs.ShareCreateDTO;

namespace StreakHub.API.Services.Interfaces
{
    public interface IShareService
    {
        Task<ShareResponseDTO> CreateShareAsync(int userId, ShareCreateDTO dto);
        Task<bool> DeleteShareAsync(int userId, int shareId);
    }
}
