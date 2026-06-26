using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreakHub.API.Services;

namespace StreakHub.API.Controllers
{
    [Route("api/tags")] // 12
    [ApiController]
    //[Authorize]
    public class TagsController : ControllerBase
    {
        private readonly TodoService _todoService;

        public TagsController(TodoService todoService)
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