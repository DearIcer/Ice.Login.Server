using System.Linq.Expressions;
using Ice.Login.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Ice.Login.Repository.Repository.Base;

public class DbRepository(IceDbContext dbContext) : BaseRepository<IceDbContext>(dbContext)
{
    public override async Task<int> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }

    public override async Task<T> Queryable<T>(Expression<Func<T, bool>> whereExpression,
        params Expression<Func<T, object>>[] includes)
    {
        var query = DbContext.Set<T>().Where(whereExpression);
        foreach (var include in includes) query = query.Include(include);
        return await query.FirstOrDefaultAsync();
    }

    public override async Task<List<T>> QueryableList<T>(Expression<Func<T, bool>> whereExpression,
        params Expression<Func<T, object>>[] includes)
    {
        var query = DbContext.Set<T>().Where(whereExpression);
        foreach (var include in includes) query = query.Include(include);
        return await query.ToListAsync();
    }

    public override async Task<(List<T> Data, int TotalCount)> GetPagedDataAsync<T>(
        Expression<Func<T, bool>> whereExpression, int pageIndex, int pageSize,
        params Expression<Func<T, object>>[] includes)
    {
        var totalCount = await DbContext.Set<T>().CountAsync(whereExpression);
        var query = DbContext.Set<T>().Where(whereExpression);
        foreach (var include in includes) query = query.Include(include);
        var data = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (data, totalCount);
    }
}