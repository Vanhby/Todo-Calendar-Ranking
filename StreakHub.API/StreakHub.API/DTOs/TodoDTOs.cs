namespace StreakHub.API.DTOs
{
    // Endpoint 7: POST /api/todos
    public class TodoCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string TaskDate { get; set; } = string.Empty;
        public List<int> TagIds { get; set; } = new();
    }

<<<<<<< HEAD
    // 8: POST /api/todos/recurring
=======
    //8: POST /api/todos/recurring
>>>>>>> feature/share-ranking
    public class TodoRecurringRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
    }

<<<<<<< HEAD
    // 9: PUT /api/todos/{id}
=======
    //  9: PUT /api/todos/{id}
>>>>>>> feature/share-ranking
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
<<<<<<< HEAD
=======

    // 12 (Lấy danh sách Tag) và gộp vào TodoResponse
    public class TagDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
>>>>>>> feature/share-ranking
}