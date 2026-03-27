using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using LexiFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LexiFlow.Infrastructure.Repositories;

public class ReviewHistoryRepository : GenericRepository<ReviewHistory>, IReviewHistoryRepository
{
    public ReviewHistoryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<ReviewHistory>> GetByUserAndDateRangeAsync(
        Guid userId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.ReviewHistories
            .AsNoTracking()
            .Include(h => h.Word)
            .ThenInclude(w => w.Category)
            .Where(h => h.UserId == userId && h.ReviewedAtUtc >= fromUtc && h.ReviewedAtUtc <= toUtc)
            .ToListAsync(cancellationToken);
    }
}
