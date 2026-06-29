namespace StreakHub.API.DTOs
{
    // Endpoint 7: POST /api/todos
    public class TodoCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string TaskDate { get; set; } = string.Empty;
        public List<int> TagIds { get; set; } = new();
    }

    public class TodoRecurringRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
    }

    public class TodoUpdateRequest
    {
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }

    // 11: GET /api/todos/day
    public class TodoResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public List<TagDTO> Tags { get; set; } = new();
    }
}