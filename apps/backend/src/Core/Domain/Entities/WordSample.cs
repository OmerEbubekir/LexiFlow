namespace LexiFlow.Domain.Entities;

/// <summary>
/// Story-2: Bir kelimeye ait örnek cümle.
/// Bir kelime birden fazla örnek cümle içerebilir (one-to-many).
/// </summary>
public sealed class WordSample : BaseEntity
{
    /// <summary>Örnek cümlenin ait olduğu kelime.</summary>
    public Guid WordId { get; private set; }
    public Word Word { get; private set; } = default!;

    /// <summary>İngilizce örnek cümle metni.</summary>
    public string SentenceText { get; private set; } = default!;

    /// <summary>
    /// Örnek cümlenin Türkçe çevirisi.
    /// Opsiyonel — girilmezse null kalır.
    /// </summary>
    public string? TurkishTranslation { get; private set; }

    // ── Constructor Metodu ──────────────────────────────────────────────────────

    private WordSample() { }

    public static WordSample Create(Guid wordId, string sentenceText, string? turkishTranslation = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sentenceText);

        return new WordSample
        {
            WordId             = wordId,
            SentenceText       = sentenceText.Trim(),
            TurkishTranslation = turkishTranslation?.Trim()
        };
    }

    public void Update(string sentenceText, string? turkishTranslation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sentenceText);
        SentenceText       = sentenceText.Trim();
        TurkishTranslation = turkishTranslation?.Trim();
        MarkUpdated();
    }
}
