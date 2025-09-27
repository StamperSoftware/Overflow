using QuestionService.DTOs;
using QuestionService.Models;

namespace QuestionService.Interfaces;

public interface IQuestionService
{
    public Task<Question> CreateQuestion(CreateQuestionDto dto, string userId, string userName);
    public Task<List<Question>> GetQuestions(string? tag);
    public Task<Question?> GetQuestion(string id);
    public Task DeleteQuestion(string id, string userId);
    public Task UpdateQuestion(string id, string userId, UpdateQuestionDto dto);
    public Task MarkAcceptedAnswer(string questionId, string answerId, string userId);
}