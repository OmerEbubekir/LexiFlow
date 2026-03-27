namespace LexiFlow.Domain.Entities;

/// <summary>
/// Story-6: Öğrenilmiş kelimeler havuzundan seçilen kelimelerle oynanan Wordle oyunu.
/// Her satır bir tahmin denemesidir; en fazla 6 deneme hakkı vardır.
/// </summary>
public sealed class WordleGame : BaseEntity
{
    // ── Oyun Verisi ─────────────────────────────────────────────────────────

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    /// <summary>
    /// Kullanıcının bulmaya çalıştığı hedef kelime (İngilizce, 5 harf).
    /// Oyun bitmeden kullanıcıya gösterilmez.
    /// </summary>
    public string TargetWord { get; private set; } = default!;

    /// <summary>
    /// Kullanıcının yaptığı tahminlerin JSON dizisi.
    /// Her eleman bir {"guess":"crane","result":"⬛🟨🟩⬛⬛"} nesnesidir.
    /// </summary>
    public string GuessesJson { get; private set; } = "[]";

    /// <summary>Maksimum deneme sayısı (sabit: 6).</summary>
    public int MaxAttempts { get; private set; } = 6;

    /// <summary>Oyun tamamlandı mı (kazanıldı veya kaybedildi)?</summary>
    public bool IsCompleted { get; private set; } = false;

    /// <summary>Oyun kazanıldı mı?</summary>
    public bool IsWon { get; private set; } = false;

    /// <summary>UTC olarak oyunun başladığı an.</summary>
    public DateTime StartedAtUtc { get; private set; }

    /// <summary>UTC olarak oyunun bittiği an. Null ise devam ediyor.</summary>
    public DateTime? CompletedAtUtc { get; private set; }

    // ── Fabrika Metodu ──────────────────────────────────────────────────────

    private WordleGame() { }

    public static WordleGame Create(Guid userId, string targetWord)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetWord);

        return new WordleGame
        {
            UserId       = userId,
            TargetWord   = targetWord.Trim().ToLowerInvariant(),
            StartedAtUtc = DateTime.UtcNow
        };
    }

    // ── Domain Davranışları ─────────────────────────────────────────────────

    /// <summary>Yeni bir tahmin ekler ve oyun durumunu günceller.</summary>
    /// <param name="updatedGuessesJson">Güncellenmiş tahminlerin JSON dizisi.</param>
    /// <param name="isWon">Bu tahminle kazanıldı mı?</param>
    /// <param name="isCompleted">Oyun tamamlandı mı (kazanıldı veya hak bitti)?</param>
    public void AddGuess(string updatedGuessesJson, bool isWon, bool isCompleted)
    {
        GuessesJson = updatedGuessesJson;
        IsWon       = isWon;
        IsCompleted = isCompleted;

        if (isCompleted)
        {
            CompletedAtUtc = DateTime.UtcNow;
        }

        MarkUpdated();
    }
}
