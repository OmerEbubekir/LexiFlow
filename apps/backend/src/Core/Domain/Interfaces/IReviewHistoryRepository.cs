using LexiFlow.Domain.Entities;

namespace LexiFlow.Domain.Interfaces;

/// <summary>Tekrar geçmişi deposu sözleşmesi.</summary>
public interface IReviewHistoryRepository : IRepository<ReviewHistory>
{
    /// <summary>
    /// Kullanıcının belirli bir zaman aralığındaki tekrar geçmişini getirir.
    /// Story-5: Analiz raporları için kullanılır.
    /// </summary>
    Task<IReadOnlyList<ReviewHistory>> GetByUserAndDateRangeAsync(
        Guid userId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default);
}
