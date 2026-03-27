namespace LexiFlow.Domain.Entities;

/// <summary>
/// Firebase üzerinden kimlik doğrulaması yapılmış platform kullanıcısı.
/// FirebaseUid, kullanıcının birden fazla cihazda aynı hesabı kullanmasını sağlar.
/// </summary>
public sealed class User : BaseEntity
{
    // ── Kimlik ──────────────────────────────────────────────────────────────

    /// <summary>Firebase Authentication UID (harici sistem ID'si).</summary>
    public string FirebaseUid { get; private set; } = default!;

    /// <summary>Kullanıcının e-posta adresi (Firebase'den alınır).</summary>
    public string Email { get; private set; } = default!;

    /// <summary>Görünen ad (Firebase DisplayName veya e-posta ön eki).</summary>
    public string DisplayName { get; private set; } = default!;

    // ── Kullanıcı Ayarları ──────────────────────────────────────────────────

    /// <summary>
    /// Kullanıcının günlük göreceği yeni kelime sayısı.
    /// Story-4: Kullanıcı bu değeri ayarlar ekranından değiştirebilir.
    /// Varsayılan: 10.
    /// </summary>
    public int DailyNewWordLimit { get; private set; } = 10;

    // ── İlişkiler ───────────────────────────────────────────────────────────

    public ICollection<Word> Words { get; private set; } = new List<Word>();
    public ICollection<UserWordProgress> WordProgresses { get; private set; } = new List<UserWordProgress>();
    public ICollection<ReviewHistory> ReviewHistories { get; private set; } = new List<ReviewHistory>();
    public ICollection<GeneratedStory> GeneratedStories { get; private set; } = new List<GeneratedStory>();
    public ICollection<WordleGame> WordleGames { get; private set; } = new List<WordleGame>();

    // ── Fabrika Metodu ──────────────────────────────────────────────────────

    private User() { } // EF Core için

    public static User Create(string firebaseUid, string email, string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firebaseUid);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return new User
        {
            FirebaseUid  = firebaseUid,
            Email        = email.ToLowerInvariant(),
            DisplayName  = string.IsNullOrWhiteSpace(displayName) ? email.Split('@')[0] : displayName,
            DailyNewWordLimit = 10
        };
    }

    // ── Domain Davranışları ─────────────────────────────────────────────────

    public void UpdateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));

        DisplayName = displayName;
        MarkUpdated();
    }

    public void UpdateDailyNewWordLimit(int limit)
    {
        if (limit is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(limit), "Daily new word limit must be between 1 and 100.");

        DailyNewWordLimit = limit;
        MarkUpdated();
    }
}
