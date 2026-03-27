using LexiFlow.Domain.Entities;

namespace LexiFlow.Domain.Interfaces;

/// <summary>
/// Kelime deposu sözleşmesi.
/// Sayfalama, filtreleme ve kullanıcıya özel sorgular burada tanımlanır.
/// </summary>
public interface IWordRepository : IRepository<Word>
{
    /// <summary>
    /// Kullanıcının kelime listesini sayfalı olarak getirir.
    /// </summary>
    Task<(IReadOnlyList<Word> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search = null,
        Guid? categoryId = null,
        bool? isLearned = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirtilen kullanıcıya ait, öğrenilmiş (IsLearned=true) ve
    /// 5 harfli kelimeleri getirir (Wordle havuzu).
    /// </summary>
    Task<IReadOnlyList<Word>> GetLearnedFiveLetterWordsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>Kelime zaten bu kullanıcıya eklenmiş mi kontrol eder.</summary>
    Task<bool> ExistsForUserAsync(Guid userId, string englishWord, CancellationToken cancellationToken = default);
}
