﻿using Contracts;
using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.DTOs;
using QuestionService.Interfaces;
using QuestionService.Models;
using Wolverine;

namespace QuestionService.Services;

public class QuestionService(QuestionDbContext db, IMessageBus bus, ITagService tagService):IQuestionService 
{
    public async Task<Question> CreateQuestion(CreateQuestionDto questionDto, string userId, string userName)
    {

        if (!await tagService.AreTagsValidAsync(questionDto.Tags)) throw new Exception("Invalid tags");
        var question = Question.CreateNewQuestion(questionDto, userId, userName);
        db.Questions.Add(question);
        await db.SaveChangesAsync();
        await bus.PublishAsync(new QuestionCreated(question.Id, question.Title, question.Content, question.CreatedAt, question.TagSlugs));
        return question;
    }

    public async Task<List<Question>> GetQuestions(string? tag)
    {
        var query = db.Questions.AsQueryable();
        if (tag is not null) query = query.Where(q => q.TagSlugs.Contains(tag));
        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }
    
    public async Task<Question?> GetQuestion(string id)
    {
        var question = db.Questions.Where(q => q.Id == id);
        await question.ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ViewCount, x => x.ViewCount + 1));
        return await question.FirstOrDefaultAsync();
    }

    public async Task UpdateQuestion(string id, string userId, UpdateQuestionDto dto)
    {
        var question = await db.Questions.FindAsync(id) ?? throw new Exception("Could not get question");
        if (!await tagService.AreTagsValidAsync(dto.Tags)) throw new Exception("Invalid tags");
        if (question.AskerId != userId) throw new Exception("Forbidden");
        question.Title = dto.Title;
        question.Content = dto.Content;
        question.TagSlugs = dto.Tags;
        question.UpdatedAt = DateTime.UtcNow;
        db.Update(question);
        await db.SaveChangesAsync();
    }

    public async Task DeleteQuestion(string id, string userId)
    {
        var question = await db.Questions.FindAsync(id) ?? throw new Exception("Could not get question");
        if (userId != question.AskerId) throw new Exception("Forbidden");
        db.Questions.Remove(question);
        await db.SaveChangesAsync();
    }

}