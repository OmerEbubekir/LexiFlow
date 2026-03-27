namespace LexiFlow.Domain.Interfaces;

/// <summary>
/// Unit of Work sözleşmesi.
/// Birden fazla repository'yi tek bir transaction altında gruplayarak
/// veri tutarlılığını sağlar.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IWordRepository Words { get; }
    IUserRepository Users { get; }
    IUserProgressRepository UserProgresses { get; }
    IReviewHistoryRepository ReviewHistories { get; }

    /// <summary>
    /// Beklemedeki tüm değişiklikleri veritabanına uygular.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Yeni bir veritabanı transaction'ı başlatır.</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Aktif transaction'ı commit eder.</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Aktif transaction'ı geri alır (rollback).</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
