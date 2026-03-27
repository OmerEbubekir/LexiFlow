using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using LexiFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LexiFlow.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext DbContext;

    public GenericRepository(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<T>().AddAsync(entity, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        DbContext.Set<T>().Update(entity);
    }

    public virtual void Delete(T entity)
    {
        DbContext.Set<T>().Remove(entity);
    }
}
