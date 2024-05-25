using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。

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