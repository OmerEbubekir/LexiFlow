using LexiFlow.Domain.Interfaces;
using LexiFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace LexiFlow.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private IDbContextTransaction? _currentTransaction;

    public IWordRepository Words { get; }
    public IUserRepository Users { get; }
    public IUserProgressRepository UserProgresses { get; }
    public IReviewHistoryRepository ReviewHistories { get; }

    public UnitOfWork(
        AppDbContext dbContext,
        IWordRepository words,
        IUserRepository users,
        IUserProgressRepository userProgresses,
        IReviewHistoryRepository reviewHistories)
    {
        _dbContext       = dbContext;
        Words            = words;
        Users            = users;
        UserProgresses   = userProgresses;
        ReviewHistories  = reviewHistories;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _dbContext.Dispose();
    }
}
