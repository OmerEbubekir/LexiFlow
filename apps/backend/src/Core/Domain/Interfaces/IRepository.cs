namespace LexiFlow.Domain.Interfaces;

/// <summary>
/// Generic repository contract.
/// Temel CRUD işlemlerini tanımlar; entity-spesifik sorgular için
/// türetilmiş interface'ler kullanılır.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
