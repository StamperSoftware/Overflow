using QuestionService.Models;

namespace QuestionService.Interfaces;

public interface ITagService
{
    public Task<IReadOnlyList<Tag>> GetTags();
}