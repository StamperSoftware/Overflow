using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.Interfaces;
using QuestionService.Models;

namespace QuestionService.Services;

public class TagService(QuestionDbContext db):ITagService
{
    public async Task<IReadOnlyList<Tag>> GetTags()
    {
        return await db.Tags.OrderByDescending(t => t.Name).ToListAsync();
    }
}