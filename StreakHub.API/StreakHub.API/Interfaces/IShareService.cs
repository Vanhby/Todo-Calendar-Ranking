using System.Threading.Tasks;
using StreakHub.API.DTOs;

namespace StreakHub.API.Services
{
    public interface IShareService
    {
        Task<ShareResponseDTO?> GetShareByIdAsync(int id);
        Task<ShareResponseDTO> CreateShareAsync(ShareCreateDTO dto);
        Task<bool> ImportSharedListAsync(int currentUserId, ImportShareDTO dto);
        Task<bool> DeleteShareAsync(int id);
    }
}