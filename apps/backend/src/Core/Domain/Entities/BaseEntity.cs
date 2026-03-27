namespace LexiFlow.Domain.Entities;

/// <summary>
/// Tüm entity'ler için ortak temel sınıf.
/// Id, oluşturma ve güncelleme tarihleri UTC olarak saklanır.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>UTC olarak kaydedilen oluşturma tarihi.</summary>
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;

    /// <summary>UTC olarak kaydedilen son güncelleme tarihi. Null ise hiç güncellenmemiştir.</summary>
    public DateTime? UpdatedAtUtc { get; protected set; }

    protected void MarkUpdated() => UpdatedAtUtc = DateTime.UtcNow;
}
