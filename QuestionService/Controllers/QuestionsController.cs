using System.Security.Claims;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.DTOs;
using QuestionService.Interfaces;
using QuestionService.Models;
using Wolverine;

namespace QuestionService.Controllers;

public class QuestionsController(IQuestionService questionService, IMessageBus bus, IAnswerService answerService)
    : BaseApiController
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

    [Authorize, HttpPost("{questionId}/answers")]
    public async Task<ActionResult<Answer>> CreateAnswer(CreateAnswerDto dto, string questionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue("name");
        if (userId is null || userName is null) return BadRequest("Could not find user");
        try
        {
            var answer = await answerService.CreateAnswer(questionId, userId, userName, dto);
            return Ok(answer);
        }
        catch
        {
            return BadRequest("Could not create answer");
        }
    }

    [Authorize, HttpPut("{questionId}/answers/{answerId}")]
    public async Task<ActionResult> UpdateAnswer(UpdateAnswerDto dto, string answerId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return BadRequest("Could not find user");
        try
        {
            await answerService.UpdateAnswer(answerId, userId, dto);
            return NoContent();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Forbidden" => Forbid(),
                _ => BadRequest(e.Message),
            };
        }
    }

    [Authorize, HttpDelete("{questionId}/answers/{answerId}")]
    public async Task<ActionResult<Answer>> DeleteAnswer(string answerId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return BadRequest("Could not get user");
        try
        {
            await answerService.DeleteAnswer(answerId, userId);
            return NoContent();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Forbidden" => Forbid(),
                _ => BadRequest(e.Message),
            };
        }
    }

    [Authorize, HttpPost("{questionId}/answers/{answerId}/accept")]
    public async Task<ActionResult> AcceptAnswer(string answerId, string questionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return BadRequest("Could not get user");

        try
        {
            await questionService.MarkAcceptedAnswer(questionId, answerId, userId);
            return NoContent();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Forbidden" => Forbid(),
                _ => BadRequest(e.Message),
            };
        }
    }
}
