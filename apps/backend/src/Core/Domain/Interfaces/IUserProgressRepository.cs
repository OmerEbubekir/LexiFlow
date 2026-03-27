using LexiFlow.Domain.Entities;

namespace LexiFlow.Domain.Interfaces;

/// <summary>
/// Kullanıcı tekrar ilerlemesi deposu sözleşmesi.
/// 6-rep algoritmasına ait sorgular burada tanımlanır.
/// </summary>
public interface IUserProgressRepository : IRepository<UserWordProgress>
{
    /// <summary>
    /// Kullanıcının bugün tekrar etmesi gereken kelimeleri getirir
    /// (NextReviewDateUtc &lt;= UtcNow AND IsLearned = false).
    /// </summary>
    Task<IReadOnlyList<UserWordProgress>> GetDueForReviewAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Kullanıcının henüz başlamamış (Stage = New) kelimelerini,
    /// belirtilen limit kadar getirir (günlük yeni kelime kotası).
    /// </summary>
    Task<IReadOnlyList<UserWordProgress>> GetNewWordsAsync(
        Guid userId,
        int limit,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli bir kullanıcı-kelime çiftinin ilerleme kaydını getirir.
    /// Yoksa null döner.
    /// </summary>
    Task<UserWordProgress?> GetByUserAndWordAsync(
        Guid userId,
        Guid wordId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Kullanıcının öğrenilmiş kelimelerini getirir (IsLearned = true).
    /// </summary>
    Task<IReadOnlyList<UserWordProgress>> GetLearnedAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
