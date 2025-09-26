using Microsoft.AspNetCore.Mvc;
using QuestionService.Interfaces;
using QuestionService.Models;

namespace QuestionService.Controllers;

public class TagsController(ITagService tagService):BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Tag>>> GetTags()
    {
        return Ok(await tagService.GetTags());
    }

}