namespace LexiFlow.Domain.Entities;

/// <summary>
/// Story-7: Gemini LLM tarafından üretilen hikaye ve görselin kalıcı kaydı.
/// Kullanıcının 5 kelimesinden yola çıkılarak hikaye + görsel oluşturulur ve kaydedilir.
/// </summary>
public sealed class GeneratedStory : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    /// <summary>Gemini'nin ürettiği hikaye metni.</summary>
    public string StoryText { get; private set; } = default!;

    /// <summary>
    /// Oluşturulmuş görselin sunucu üzerindeki dosya yolu veya URL'i.
    /// Image Generation API tarafından döndürülen görsel buraya kaydedilir.
    /// </summary>
    public string? ImagePath { get; private set; }

    /// <summary>
    /// Hikayede kullanılan kelimelerin JSON dizisi.
    /// Örnek: ["serendipity","ephemeral","melancholy","resilient","ethereal"]
    /// </summary>
    public string WordsUsedJson { get; private set; } = "[]";

    /// <summary>UTC olarak üretim anı.</summary>
    public DateTime GeneratedAtUtc { get; private set; }

    // ── Fabrika Metodu ──────────────────────────────────────────────────────

    private GeneratedStory() { }

    public static GeneratedStory Create(
        Guid userId,
        string storyText,
        string wordsUsedJson,
        string? imagePath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storyText);

        return new GeneratedStory
        {
            UserId        = userId,
            StoryText     = storyText,
            WordsUsedJson = wordsUsedJson,
            ImagePath     = imagePath,
            GeneratedAtUtc = DateTime.UtcNow
        };
    }

    /// <summary>Görsel oluşturulduktan sonra ImagePath güncellemek için.</summary>
    public void SetImagePath(string imagePath)
    {
        ImagePath = imagePath;
        MarkUpdated();
    }
}
