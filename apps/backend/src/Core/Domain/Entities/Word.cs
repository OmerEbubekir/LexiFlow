using LexiFlow.Domain.Enums;

namespace LexiFlow.Domain.Entities;

/// <summary>
/// Story-2: Gelişmiş Kelime Modülü.
/// İngilizce kelime, Türkçe karşılığı, resim yolu, ses dosyası, zorluk seviyesi ve
/// kategori bilgilerini barındırır. Örnek cümleler ayrı <see cref="WordSample"/> tablosunda tutulur.
/// </summary>
public sealed class Word : BaseEntity
{
    // ── Kelime Verileri ─────────────────────────────────────────────────────

    /// <summary>İngilizce kelime (örn: "serendipity").</summary>
    public string EnglishWord { get; private set; } = default!;

    /// <summary>Türkçe karşılığı (örn: "tesadüfen güzel şeyler bulma yeteneği").</summary>
    public string TurkishTranslation { get; private set; } = default!;

    /// <summary>
    /// Story-2: Kelimenin görselini temsil eden dosya yolu veya URL.
    /// Opsiyonel.
    /// </summary>
    public string? PictureUrl { get; private set; }

    /// <summary>
    /// Story-2: Kelimenin sesli okunuşunun dosya yolu veya URL'i.
    /// Opsiyonel.
    /// </summary>
    public string? AudioUrl { get; private set; }

    /// <summary>Kelimenin zorluk seviyesi (1-5 arası).</summary>
    public DifficultyLevel DifficultyLevel { get; private set; } = DifficultyLevel.Medium;

    // ── İlişkiler ───────────────────────────────────────────────────────────

    /// <summary>Bu kelimeyi ekleyen kullanıcının ID'si.</summary>
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    /// <summary>Kelimenin ait olduğu kategori. Opsiyonel.</summary>
    public Guid? CategoryId { get; private set; }
    public Category? Category { get; private set; }

    /// <summary>Story-2: Kelimeye ait örnek cümleler (ayrı tablo).</summary>
    public ICollection<WordSample> Samples { get; private set; } = new List<WordSample>();

    /// <summary>Kullanıcının bu kelime üzerindeki tekrar ilerlemesi.</summary>
    public ICollection<UserWordProgress> Progresses { get; private set; } = new List<UserWordProgress>();

    // ── Fabrika Metodu ──────────────────────────────────────────────────────

    private Word() { }

    public static Word Create(
        Guid userId,
        string englishWord,
        string turkishTranslation,
        DifficultyLevel difficultyLevel = DifficultyLevel.Medium,
        Guid? categoryId = null,
        string? pictureUrl = null,
        string? audioUrl = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(englishWord);
        ArgumentException.ThrowIfNullOrWhiteSpace(turkishTranslation);

        return new Word
        {
            UserId             = userId,
            EnglishWord        = englishWord.Trim().ToLowerInvariant(),
            TurkishTranslation = turkishTranslation.Trim(),
            DifficultyLevel    = difficultyLevel,
            CategoryId         = categoryId,
            PictureUrl         = pictureUrl,
            AudioUrl           = audioUrl
        };
    }

    // ── Domain Davranışları ─────────────────────────────────────────────────

    public void Update(
        string englishWord,
        string turkishTranslation,
        DifficultyLevel difficultyLevel,
        Guid? categoryId,
        string? pictureUrl,
        string? audioUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(englishWord);
        ArgumentException.ThrowIfNullOrWhiteSpace(turkishTranslation);

        EnglishWord        = englishWord.Trim().ToLowerInvariant();
        TurkishTranslation = turkishTranslation.Trim();
        DifficultyLevel    = difficultyLevel;
        CategoryId         = categoryId;
        PictureUrl         = pictureUrl;
        AudioUrl           = audioUrl;
        MarkUpdated();
    }
}
