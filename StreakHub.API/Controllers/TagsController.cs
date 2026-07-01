using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreakHub.API.Interfaces; 

namespace StreakHub.API.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITodoService _todoService; 

        public TagsController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _todoService.GetAllTagsAsync();
            return Ok(tags);
        }
    }
}