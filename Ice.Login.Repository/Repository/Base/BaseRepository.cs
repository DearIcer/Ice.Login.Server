using System.Linq.Expressions;
using Ice.Login.Repository.Extensions;
using Ice.Login.Repository.IRepository.Base;
using Microsoft.EntityFrameworkCore;
using Share;

#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。

#pragma warning disable CS8603 // 可能返回 null 引用。
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Ice.Login.Repository.Repository.Base;

public abstract class BaseRepository<TContext> : IBaseRepository, IUnitOfWork where TContext : DbContext
{
    protected BaseRepository(TContext dbContext)
    {
        DbContext = dbContext;
    }

    protected TContext DbContext { get; }

    public async Task<T> Queryable<T>(Expression<Func<T, bool>> whereExpression,
        params Expression<Func<T, object>>[] includes) where T : class
    {
        var data = await DbContext.Set<T>().GetAsync(whereExpression, includes);
        return data;
    }

    public async Task<List<T>> QueryableList<T>(Expression<Func<T, bool>> whereExpression,
        params Expression<Func<T, object>>[] includes) where T : class
    {
        var data = await DbContext.Set<T>().GetListAsync(whereExpression, includes);
        return data;
    }

    public async Task<(List<T> Data, int TotalCount)> GetPagedDataAsync<T>(Expression<Func<T, bool>> whereExpression,
        int pageIndex, int pageSize,
        Expression<Func<T, object>> orderByExpression = default,
        params Expression<Func<T, object>>[] includes) where T : class
    {
        var data = await DbContext.Set<T>()
            .GetListToPageAsync(whereExpression, pageIndex, pageSize, orderByExpression, includes);
        return (data.List, data.Total);
    }


    public async Task<(List<T> Data, int TotalCount)> GetPagedDataWithFilterAsync<T>(
        Expression<Func<T, bool>> whereExpression, Expression<Func<T, bool>> filterExpression,
        int pageIndex, int pageSize,
        Expression<Func<T, object>> orderByExpression = default,
        params Expression<Func<T, object>>[] includes) where T : class
    {
        var data = await DbContext.Set<T>().GetListToPageAsync(whereExpression, pageIndex, pageSize, filterExpression,
            orderByExpression, includes);
        return (data.List, data.Total);
    }

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
    
}