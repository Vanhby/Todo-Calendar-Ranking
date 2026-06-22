namespace StreakHub.API.DTOs
{
    public class CalendarDayDTO
    {
        public DateOnly Date { get; set; }
        public int TotalTasks => Tasks.Count;
        public int CompletedTasks => Tasks.Count(t => t.IsCompleted);

        public double CompletionPercentage => TotalTasks == 0 ? 0 : Math.Round((double)CompletedTasks / TotalTasks * 100, 2);

        public int ColorLevel
        {
            get
            {
                if (TotalTasks == 0) return 0;
                if (CompletionPercentage == 0) return 1;
                if (CompletionPercentage < 50) return 2;
                if (CompletionPercentage < 100) return 3;
                return 4;
            }
        }

        public List<TodoItemDTO> Tasks { get; set; } = new List<TodoItemDTO>();
    }
}