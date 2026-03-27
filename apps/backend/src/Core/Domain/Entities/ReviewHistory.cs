using LexiFlow.Domain.Enums;

namespace LexiFlow.Domain.Entities;

/// <summary>
/// Her tekrar denemesinin denetim kaydı (audit log).
/// Quiz analizleri ve başarı oranı raporları için kullanılır.
/// </summary>
public sealed class ReviewHistory : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid WordId { get; private set; }
    public Word Word { get; private set; } = default!;

    /// <summary>Cevap doğru muydu?</summary>
    public bool IsCorrect { get; private set; }

    /// <summary>UTC olarak tekrar yapılan an.</summary>
    public DateTime ReviewedAtUtc { get; private set; }

    /// <summary>
    /// Cevap verilmeden önceki aşama.
    /// Analytics: Hangi aşamada en çok hata yapıldığını gösterir.
    /// </summary>
    public RepetitionStage StageBeforeReview { get; private set; }

    // ── Fabrika Metodu ──────────────────────────────────────────────────────

    private ReviewHistory() { }

    public static ReviewHistory Create(
        Guid userId,
        Guid wordId,
        bool isCorrect,
        RepetitionStage stageBeforeReview)
    {
        return new ReviewHistory
        {
            UserId             = userId,
            WordId             = wordId,
            IsCorrect          = isCorrect,
            ReviewedAtUtc      = DateTime.UtcNow,
            StageBeforeReview  = stageBeforeReview
        };
    }
}
