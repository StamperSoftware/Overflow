using Contracts;
using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.DTOs;
using QuestionService.Interfaces;
using QuestionService.Models;
using Wolverine;

namespace QuestionService.Services;

public class AnswerService(QuestionDbContext db, IMessageBus bus):IAnswerService
{
    public async Task<List<Answer>> GetAnswers()
    {
        return await db.Answers.ToListAsync();
    }

    public async Task<Answer> GetAnswer(string answerId)
    {
        return await db.Answers.FirstOrDefaultAsync(a => a.Id == answerId) ?? throw new Exception("Could not get answer");
    }

    public async Task<Answer> CreateAnswer(string questionId, string userId, string userDisplayName, CreateAnswerDto dto)
    {
        var question = await db.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.Id == questionId) ?? throw new Exception("Could not get question.");
        var answer = Answer.Create(dto.Content, userId, userDisplayName, questionId);
        question.Answers.Add(answer);
        question.AnswerCount++;
        await db.SaveChangesAsync();
        await bus.PublishAsync(new AnswerCountUpdated(question.Id, question.AnswerCount));
        return answer;
    }

    public async Task UpdateAnswer(string answerId, string userId, UpdateAnswerDto dto)
    {
        var answer = await db.Answers.FindAsync(answerId) ?? throw new Exception("Could not get answer");
        if (userId != answer.UserId) throw new Exception("Forbidden");
        answer.Content = dto.Content;
        db.Update(answer);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAnswer(string answerId, string userId)
    {
        var answer = await db.Answers.FindAsync(answerId) ?? throw new Exception("Could not get answer");
        var question = await db.Questions.FindAsync(answer.QuestionId) ?? throw new Exception("Could not get question");
        if (userId != answer.UserId) throw new Exception("Forbidden");
        db.Remove(answer);
        question.AnswerCount--;
        await db.SaveChangesAsync();
        await bus.PublishAsync(new AnswerCountUpdated(question.Id, question.AnswerCount));
    }
}