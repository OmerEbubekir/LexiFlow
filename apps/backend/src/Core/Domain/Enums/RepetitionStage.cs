namespace LexiFlow.Domain.Enums;

/// <summary>
/// 6 Sefer Tekrar Prensibi'nin aşamalarını tanımlar.
/// Her aşama, bir sonraki tekrar için kaç gün bekleneceğini belirler.
/// </summary>
public enum RepetitionStage
{
    /// <summary>Yeni eklenen, henüz hiç çalışılmamış kelime.</summary>
    New = 0,

    /// <summary>1. doğru → 1 gün sonra tekrar (NextReviewDate += 1 gün)</summary>
    Day1 = 1,

    /// <summary>2. doğru → 7 gün sonra tekrar (NextReviewDate += 7 gün)</summary>
    Week1 = 2,

    /// <summary>3. doğru → 30 gün sonra tekrar (NextReviewDate += 30 gün)</summary>
    Month1 = 3,

    /// <summary>4. doğru → 90 gün sonra tekrar (NextReviewDate += 90 gün)</summary>
    Month3 = 4,

    /// <summary>5. doğru → 180 gün sonra tekrar (NextReviewDate += 180 gün)</summary>
    Month6 = 5,

    /// <summary>6. doğru → 365 gün sonra tekrar (NextReviewDate += 365 gün) — ardından IsLearned = true</summary>
    Year1 = 6,

    /// <summary>6/6 tamamlandı. Kelime "Bilinen Kelimeler" havuzuna taşındı.</summary>
    Learned = 7
}
