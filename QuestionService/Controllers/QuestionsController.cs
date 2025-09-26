using System.Security.Claims;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.DTOs;
using QuestionService.Interfaces;
using QuestionService.Models;
using Wolverine;

namespace QuestionService.Controllers;

public class QuestionsController(IQuestionService questionService, IMessageBus bus):BaseApiController
{
    [Authorize, HttpPost]
    public async Task<ActionResult<Question>> CreateQuestion(CreateQuestionDto questionDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = User.FindFirstValue("name");
        if (userId is null || name is null) return BadRequest("Cannot get user");
        try
        {
            var question = await questionService.CreateQuestion(questionDto, userId, name);
            return Created($"/questions/{question.Id}", question);
        }
        catch (Exception e)
        {
            return BadRequest($"Could not create question, {e.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<Question>>> GetQuestions(string? tag)
    {
        return await questionService.GetQuestions(tag);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestion(string id)
    {
        var question = await questionService.GetQuestion(id);
        if (question is null) return NotFound();
        return Ok(question);
    }

    [Authorize, HttpPut("{id}")]
    public async Task<ActionResult> UpdateQuestion(string id, UpdateQuestionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return BadRequest("Could not get user");
        try
        {
            await questionService.UpdateQuestion(id, userId, dto);
            await bus.PublishAsync(new QuestionUpdated(id, dto.Title, dto.Content, dto.Tags.ToArray()));
            return NoContent();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Invalid tags" => BadRequest("Invalid Tags"),
                "Forbidden" => Forbid(),
                _ => BadRequest("unknown error occurred")
            };
        }
    }

    [Authorize, HttpDelete("{id}")]
    public async Task<ActionResult> DeleteQuestion(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return BadRequest("Could not get user");
        try
        {
            await questionService.DeleteQuestion(id, userId);
            await bus.PublishAsync(new QuestionDeleted(id));
            return Ok();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Forbidden" => Forbid(),
                _ => BadRequest("could not delete question"),
            };
        }
    }
}