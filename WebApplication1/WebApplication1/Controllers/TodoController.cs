using Microsoft.AspNetCore.Mvc;

namespace Todo_Calendar_Ranking.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Đường dẫn sẽ là: api/todo
    public class TodoController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTasks()
        {
            var tasks = new List<string> { "Học .NET 10", "Fix lỗi Git", "Thiết kế DB" };
            return Ok(tasks);
        }
    }
}
