namespace StreakHub.API.DTOs
{
    public class TodoDTOs
    {
        // Dùng cho 5 
        public class TodoCreateRequest
        {
            public string Title { get; set; } = string.Empty;
            public string Date { get; set; } = string.Empty;
        }

        //7 
        public class TodoUpdateRequest
        {
            public string Title { get; set; } = string.Empty;
            public bool IsCompleted { get; set; }
        }

      //9 
        public class TodoResponse
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public bool IsCompleted { get; set; }
        }
    }
}
