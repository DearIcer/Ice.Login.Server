using System.Linq.Expressions;
using Ice.Login.Repository.IRepository.Base;
using Microsoft.EntityFrameworkCore;
using Share;

namespace Ice.Login.Repository.Repository.Base;

public abstract class BaseRepository<TContext> : IBaseRepository, IUnitOfWork where TContext : DbContext
{
    protected BaseRepository(TContext dbContext)
    {
        DbContext = dbContext;
    }

    protected TContext DbContext { get; }

    public abstract Task<int> SaveChangesAsync();

    async Task IUnitOfWork.BeginTransaction()
    {
        await DbContext.Database.BeginTransactionAsync();
    }

    async Task IUnitOfWork.CommitTransaction()
    {
        await DbContext.Database.CommitTransactionAsync();
    }

    async Task IUnitOfWork.RollbackTransaction()
    {
        await DbContext.Database.RollbackTransactionAsync();
    }

    public abstract Task<T> Queryable<T>(Expression<Func<T, bool>> whereExpression,
        params Expression<Func<T, object>>[] includes) where T : class;

    public abstract Task<List<T>> QueryableList<T>(Expression<Func<T, bool>> whereExpression,
        params Expression<Func<T, object>>[] includes) where T : class;

    public abstract Task<(List<T> Data, int TotalCount)> GetPagedDataAsync<T>(Expression<Func<T, bool>> whereExpression,
        int pageIndex, int pageSize, params Expression<Func<T, object>>[] includes) where T : class;
}