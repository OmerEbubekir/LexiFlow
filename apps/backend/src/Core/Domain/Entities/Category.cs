namespace LexiFlow.Domain.Entities;

/// <summary>
/// Kelime kategorisi / konusu.
/// Story-5: Analiz raporları kategorilere göre gruplanır.
/// </summary>
public sealed class Category : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    // ── İlişkiler ───────────────────────────────────────────────────────────

    public ICollection<Word> Words { get; private set; } = new List<Word>();

    // ── Fabrika Metodu ──────────────────────────────────────────────────────

    private Category() { }

    public static Category Create(string name, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Category
        {
            Name        = name.Trim(),
            Description = description?.Trim()
        };
    }

    public void Update(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name        = name.Trim();
        Description = description?.Trim();
        MarkUpdated();
    }
}
