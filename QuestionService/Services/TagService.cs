using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QuestionService.Data;
using QuestionService.Interfaces;
using QuestionService.Models;

namespace QuestionService.Services;

public class TagService(QuestionDbContext db, IMemoryCache cache):ITagService
{
    private const string CacheKey = "tags";
    
    public async Task<IReadOnlyList<Tag>> GetTags()
    {
        return await _getTags();
    }

    public async Task<bool> AreTagsValidAsync(List<string> slugs)
    {
        var tags = await _getTags();
        var tagSet = tags.Select(t => t.Slug).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return slugs.All(s => tagSet.Contains(s));
    }
    
    private async Task<List<Tag>> _getTags()
    {
        return await cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
            var tags = await db.Tags.AsNoTracking().OrderByDescending(t => t.Name).ToListAsync();
            return tags;
        }) ?? []; 
    }
}