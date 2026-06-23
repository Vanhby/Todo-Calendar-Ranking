using System.Threading.Tasks;
using static StreakHub.API.DTOs.ShareDTO;

namespace StreakHub.API.Services
{
    public interface IShareService
    {
        Task<ShareResponseDTO?> GetShareByIdAsync(int id);
        Task<ShareResponseDTO> CreateShareAsync(ShareCreateDTO dto);
        Task<bool> DeleteShareAsync(int id);
    }
}