using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using LexiFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LexiFlow.Infrastructure.Repositories;

public class WordRepository : GenericRepository<Word>, IWordRepository
{
    public WordRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<(IReadOnlyList<Word> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search = null,
        Guid? categoryId = null,
        bool? isLearned = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbContext.Words
            .Include(w => w.Samples)
            .Include(w => w.Category)
            .Where(w => w.UserId == userId)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(w => w.EnglishWord.Contains(s) || w.TurkishTranslation.ToLower().Contains(s));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(w => w.CategoryId == categoryId.Value);
        }

        // Apply isLearned filter via UserWordProgress (if available)
        if (isLearned.HasValue)
        {
            query = query.Where(w => w.Progresses.Any(p => p.UserId == userId && p.IsLearned == isLearned.Value));
            // Alternatively if we assume w.UserId == userId applies to all progresses for this word:
            // But w.UserId = original creator. 
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(w => w.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Word>> GetLearnedFiveLetterWordsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Words
            .AsNoTracking()
            .Where(w => w.EnglishWord.Length == 5)
            .Where(w => w.Progresses.Any(p => p.UserId == userId && p.IsLearned == true))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, string englishWord, CancellationToken cancellationToken = default)
    {
        var e = englishWord.ToLowerInvariant();
        return await DbContext.Words.AnyAsync(w => w.UserId == userId && w.EnglishWord == e, cancellationToken);
    }
}
