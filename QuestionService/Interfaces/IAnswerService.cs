using QuestionService.DTOs;
using QuestionService.Models;

namespace QuestionService.Interfaces;

public interface IAnswerService
{
    public Task<List<Answer>> GetAnswers();
    public Task<Answer> GetAnswer(string answerId);
    public Task<Answer> CreateAnswer(string questionId, string userId, string userDisplayName, CreateAnswerDto dto);
    public Task UpdateAnswer(string answerId, string userId, UpdateAnswerDto dto);
    public Task DeleteAnswer(string answerId, string userId);
}