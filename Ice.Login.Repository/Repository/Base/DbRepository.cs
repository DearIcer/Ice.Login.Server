using Ice.Login.Repository.Context;
using Microsoft.Extensions.Logging;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
#pragma warning disable CS8603 // 可能返回 null 引用。

namespace Ice.Login.Repository.Repository.Base;

public class DbRepository(IceDbContext dbContext, ILogger<DbRepository> logger)
    : BaseRepository<IceDbContext>(dbContext)
{
    public override async Task<int> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }

    // private IQueryable<T> BuildQuery<T>(Expression<Func<T, bool>> whereExpression,
    //     IEnumerable<Expression<Func<T, object>>> includes) where T : class
    // {
    //     var query = DbContext.Set<T>().Where(whereExpression);
    //     foreach (var include in includes) query = query.Include(include);
    //     return query;
    // }

    // public override async Task<T> Queryable<T>(Expression<Func<T, bool>> whereExpression,
    //     params Expression<Func<T, object>>[] includes)
    // {
    //     try
    //     {
    //         var query = BuildQuery<T>(whereExpression, includes);
    //         return await query.FirstOrDefaultAsync();
    //     }
    //     catch (Exception e)
    //     {
    //         LogError(e);
    //         throw;
    //     }
    // }
    //
    // public override async Task<List<T>> QueryableList<T>(Expression<Func<T, bool>> whereExpression,
    //     params Expression<Func<T, object>>[] includes)
    // {
    //     try
    //     {
    //         var query = BuildQuery<T>(whereExpression, includes);
    //         return await query.ToListAsync();
    //     }
    //     catch (Exception e)
    //     {
    //         LogError(e);
    //         throw;
    //     }
    // }
    //
    // public override async Task<(List<T> Data, int TotalCount)> GetPagedDataAsync<T>(
    //     Expression<Func<T, bool>> whereExpression, int pageIndex, int pageSize,
    //     params Expression<Func<T, object>>[] includes)
    // {
    //     if (pageIndex < 1 || pageSize <= 0)
    //     {
    //         throw new ArgumentException("Invalid pageIndex or pageSize values.");
    //     }
    //     try
    //     {
    //         var totalCount = await DbContext.Set<T>().CountAsync(whereExpression);
    //         var query = BuildQuery<T>(whereExpression, includes);
    //         var data = await query
    //             .Skip((pageIndex - 1) * pageSize)
    //             .Take(pageSize)
    //             .ToListAsync();
    //
    //         return (data, totalCount);
    //     }
    //     catch (Exception e)
    //     {
    //         LogError(e);
    //         throw;
    //     }
    // }
    //
    // public override async Task<(List<T> Data, int TotalCount)> GetPagedDataWithFilterAsync<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, bool>> filterExpression, int pageIndex,
    //     int pageSize, params Expression<Func<T, object>>[] includes)
    // {
    //     try
    //     {
    //         // 应用whereExpression和includes构建初始查询
    //         var baseQuery = DbContext.Set<T>().Where(whereExpression);
    //     
    //         // 计算总条数时，先应用whereExpression，但不应用filterExpression，因为filterExpression是用于分页数据上的额外过滤
    //         var totalCount = await baseQuery.CountAsync();
    //
    //         // 构建最终查询，应用whereExpression、filterExpression和includes
    //         var finalQuery = BuildQuery<T>(whereExpression, includes);
    //         finalQuery = finalQuery.Where(filterExpression);
    //
    //         // 分页处理
    //         var data = await finalQuery
    //             .Skip((pageIndex - 1) * pageSize)
    //             .Take(pageSize)
    //             .ToListAsync();
    //
    //         return (data, totalCount);
    //     }
    //     catch (Exception e)
    //     {
    //         LogError(e);
    //         throw;
    //     }
    // }


    private void LogError(Exception ex)
    {
        logger.LogError(ex.ToString());
    }
}