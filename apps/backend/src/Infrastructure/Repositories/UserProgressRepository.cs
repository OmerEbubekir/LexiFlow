using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using LexiFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LexiFlow.Infrastructure.Repositories;

public class UserProgressRepository : GenericRepository<UserWordProgress>, IUserProgressRepository
{
    public UserProgressRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<UserWordProgress>> GetDueForReviewAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await DbContext.UserWordProgresses
            .Include(p => p.Word)
            .ThenInclude(w => w.Samples)
            .Where(p => p.UserId == userId && !p.IsLearned && p.NextReviewDateUtc.Date <= now.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserWordProgress>> GetNewWordsAsync(
        Guid userId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        // Select words created by user or assigned to user progress where stage is New
        return await DbContext.UserWordProgresses
            .Include(p => p.Word)
            .ThenInclude(w => w.Samples)
            .Where(p => p.UserId == userId && p.Stage == Domain.Enums.RepetitionStage.New && !p.IsLearned)
            .OrderBy(p => p.CreatedAtUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserWordProgress?> GetByUserAndWordAsync(
        Guid userId,
        Guid wordId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.UserWordProgresses
            .Include(p => p.Word)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.WordId == wordId, cancellationToken);
    }

    public async Task<IReadOnlyList<UserWordProgress>> GetLearnedAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.UserWordProgresses
            .Include(p => p.Word)
            .Where(p => p.UserId == userId && p.IsLearned)
            .ToListAsync(cancellationToken);
    }
}
