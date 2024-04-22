using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Share;

public interface IUnitOfWork
{
    Task BeginTransaction();
    Task CommitTransaction();
    Task RollbackTransaction();
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : DbContext, IUnitOfWork
{
    private IDbContextTransaction _transaction;

    public virtual async Task BeginTransaction()
    {
        _transaction = await Database.BeginTransactionAsync();
    }

    public virtual async Task CommitTransaction()
    {
        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public virtual async Task RollbackTransaction()
    {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await SaveChangesAsync();
    }
}