using LexiFlow.Domain.Enums;

namespace LexiFlow.Domain.Entities;

/// <summary>
/// 6 Sefer Tekrar Prensibi: Bir kullanıcının belirli bir kelime üzerindeki
/// tekrar ilerlemesini takip eder.
///
/// İş Kuralları:
/// - Doğru cevap → ConsecutiveCorrect artar, Stage bir üst aşamaya geçer, NextReviewDate güncellenir.
/// - Yanlış cevap → ConsecutiveCorrect sıfırlanır, Stage = New, NextReviewDate = UtcNow (hemen tekrar).
/// - Stage = Learned (6/6 tamamlandı) → IsLearned = true, "Bilinen Kelimeler" havuzuna alınır.
/// - IsLearned = true kelimeler Wordle havuzuna dahil edilir.
/// </summary>
public sealed class UserWordProgress : BaseEntity
{
    // ── Kimlik ──────────────────────────────────────────────────────────────

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid WordId { get; private set; }
    public Word Word { get; private set; } = default!;

    // ── 6-Rep Algoritması Durumu ─────────────────────────────────────────────

    /// <summary>Şu anda üst üste kaç kez doğru bilindiği (0-6 arası).</summary>
    public int ConsecutiveCorrect { get; private set; } = 0;

    /// <summary>
    /// Mevcut tekrar aşaması. Başlangıçta <see cref="RepetitionStage.New"/>.
    /// Her doğru cevapla bir aşama ilerler; yanlış cevapla New'e döner.
    /// </summary>
    public RepetitionStage Stage { get; private set; } = RepetitionStage.New;

    /// <summary>
    /// UTC olarak bir sonraki tekrar tarihi.
    /// Bu tarih geçmişte kalan veya bugüne eşit olan kelimeler quiz'e dahil edilir.
    /// </summary>
    public DateTime NextReviewDateUtc { get; private set; } = DateTime.UtcNow;

    /// <summary>6/6 tamamlandıysa true. "Bilinen Kelimeler" havuzundaki kelimeler.</summary>
    public bool IsLearned { get; private set; } = false;

    /// <summary>Son tekrar yapılan UTC tarihi.</summary>
    public DateTime? LastReviewedAtUtc { get; private set; }

    // ── Fabrika Metodu ──────────────────────────────────────────────────────

    private UserWordProgress() { }

    public static UserWordProgress Create(Guid userId, Guid wordId)
    {
        return new UserWordProgress
        {
            UserId              = userId,
            WordId              = wordId,
            Stage               = RepetitionStage.New,
            ConsecutiveCorrect  = 0,
            NextReviewDateUtc   = DateTime.UtcNow,
            IsLearned           = false
        };
    }

    // ── Domain Davranışları ─────────────────────────────────────────────────

    /// <summary>
    /// 6 Sefer Tekrar algoritmasını uygular.
    /// Doğru cevap → aşama ilerler. Yanlış → başa döner.
    /// </summary>
    /// <param name="isCorrect">Kullanıcının cevabı doğru mu?</param>
    public void ApplyAnswer(bool isCorrect)
    {
        LastReviewedAtUtc = DateTime.UtcNow;

        if (isCorrect)
        {
            ApplyCorrectAnswer();
        }
        else
        {
            ApplyWrongAnswer();
        }

        MarkUpdated();
    }

    private void ApplyCorrectAnswer()
    {
        ConsecutiveCorrect++;

        // Stage zaten Learned ise değişiklik yapma
        if (Stage == RepetitionStage.Learned) return;

        Stage = Stage switch
        {
            RepetitionStage.New    => RepetitionStage.Day1,
            RepetitionStage.Day1   => RepetitionStage.Week1,
            RepetitionStage.Week1  => RepetitionStage.Month1,
            RepetitionStage.Month1 => RepetitionStage.Month3,
            RepetitionStage.Month3 => RepetitionStage.Month6,
            RepetitionStage.Month6 => RepetitionStage.Year1,
            RepetitionStage.Year1  => RepetitionStage.Learned,
            _                      => Stage
        };

        NextReviewDateUtc = Stage switch
        {
            RepetitionStage.Day1    => DateTime.UtcNow.AddDays(1),
            RepetitionStage.Week1   => DateTime.UtcNow.AddDays(7),
            RepetitionStage.Month1  => DateTime.UtcNow.AddDays(30),
            RepetitionStage.Month3  => DateTime.UtcNow.AddDays(90),
            RepetitionStage.Month6  => DateTime.UtcNow.AddDays(180),
            RepetitionStage.Year1   => DateTime.UtcNow.AddDays(365),
            RepetitionStage.Learned => DateTime.UtcNow.AddDays(365), // Artık quiz'e girmez
            _                       => DateTime.UtcNow
        };

        if (Stage == RepetitionStage.Learned)
        {
            IsLearned = true;
        }
    }

    private void ApplyWrongAnswer()
    {
        // İş Kuralı: Herhangi bir aşamada yanlış → sıfırla, hemen tekrar
        ConsecutiveCorrect = 0;
        Stage              = RepetitionStage.New;
        NextReviewDateUtc  = DateTime.UtcNow;
        IsLearned          = false;
    }

    /// <summary>Bu ilerleme kaydının bugün tekrar edilmesi gerekiyor mu?</summary>
    public bool IsDueForReview() =>
        !IsLearned && NextReviewDateUtc.Date <= DateTime.UtcNow.Date;
}
