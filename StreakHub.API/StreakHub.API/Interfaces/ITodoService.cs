using StreakHub.API.DTOs;

namespace StreakHub.API.Interfaces
{
    public interface ITodoService
    {
        Task<int> CreateSingleTaskAsync(int userId, TodoCreateRequest request, DateOnly clientToday);
        Task<int> CreateRecurringTasksAsync(int userId, TodoRecurringRequest request, DateOnly clientToday);
        Task UpdateTaskAsync(int todoId, int userId, TodoUpdateRequest request);
        Task DeleteTaskAsync(int todoId, int userId);
        Task<List<TodoResponse>> GetTasksByDayAsync(int userId, DateOnly date);
        Task<List<TagDTO>> GetAllTagsAsync();
    }
}
