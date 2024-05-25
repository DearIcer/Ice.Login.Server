using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

#pragma warning disable CS8603 // 可能返回 null 引用。
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Ice.Login.Repository.Extensions;

public static class DbSetExtensions
{
    public static async Task<List<T>> GetListAsync<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> whereExpression,
        params Expression<Func<T, object>>[] includes)
        where T : class
    {
        try
        {
            var query = BuildQuery(dbSet, whereExpression, includes);
            return await query.ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public static async Task<(int Total, List<T> List)> GetListToPageAsync<T>(this DbSet<T> dbSet,
        Expression<Func<T, bool>> whereExpression, int pageIndex, int pageSize,
        Expression<Func<T, object>> orderByExpression, params Expression<Func<T, object>>[] includes)
        where T : class
    {
        if (pageIndex < 1 || pageSize <= 0) throw new ArgumentException("Invalid pageIndex or pageSize values.");
        try
        {
            var query = BuildQuery(dbSet, whereExpression, includes);
            var totalCount = await query.CountAsync();

            query = orderByExpression == default
                ? query.Skip(pageSize * pageIndex).Take(pageSize)
                : query.OrderBy(orderByExpression).Skip(pageSize * pageIndex).Take(pageSize);
            var resultList = await query.ToListAsync();

            return (totalCount, resultList);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public static async Task<(int Total, List<T> List)> GetListToPageAsync<T>(this DbSet<T> dbSet,
        Expression<Func<T, bool>> whereExpression, int pageIndex, int pageSize,
        Expression<Func<T, bool>> filterExpression, Expression<Func<T, object>> orderByExpression,
        params Expression<Func<T, object>>[] includes)
        where T : class
    {
        if (pageIndex < 1 || pageSize <= 0) throw new ArgumentException("Invalid pageIndex or pageSize values.");
        try
        {
            var baseQuery = dbSet.Where(whereExpression);
            var totalCount = await baseQuery.CountAsync();
            var finalQuery = BuildQuery(dbSet, whereExpression, includes);
            finalQuery = finalQuery.Where(filterExpression);
            finalQuery = orderByExpression == default
                ? finalQuery.Skip(pageSize * pageIndex).Take(pageSize)
                : finalQuery.OrderBy(orderByExpression).Skip(pageSize * pageIndex).Take(pageSize);
            var resultList = await finalQuery.ToListAsync();

            return (totalCount, resultList);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public static async Task<T> GetAsync<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> whereExpression,
        params Expression<Func<T, object>>[] includes)
        where T : class
    {
        try
        {
            var query = BuildQuery(dbSet, whereExpression, includes);
            return await query.FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private static IQueryable<T> BuildQuery<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> whereExpression,
        IEnumerable<Expression<Func<T, object>>> includes)
        where T : class
    {
        var query = dbSet.Where(whereExpression);
        foreach (var include in includes) query = query.Include(include);
        return query;
    }
}