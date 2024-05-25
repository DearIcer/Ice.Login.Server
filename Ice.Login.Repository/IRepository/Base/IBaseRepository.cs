using System.Linq.Expressions;
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace Ice.Login.Repository.IRepository.Base;

public interface IBaseRepository
{
    Task<T> Queryable<T>(Expression<Func<T, bool>> whereExpression, params Expression<Func<T, object>>[] includes)
        where T : class;

    Task<List<T>> QueryableList<T>(Expression<Func<T, bool>> whereExpression,
        params Expression<Func<T, object>>[] includes)
        where T : class;

    Task<(List<T> Data, int TotalCount)> GetPagedDataAsync<T>(Expression<Func<T, bool>> whereExpression, int pageIndex,
        int pageSize,
        Expression<Func<T, object>> orderByExpression = default,
        params Expression<Func<T, object>>[] includes)
        where T : class;

    Task<(List<T> Data, int TotalCount)> GetPagedDataWithFilterAsync<T>(Expression<Func<T, bool>> whereExpression,
        Expression<Func<T, bool>> filterExpression,
        int pageIndex, int pageSize,
        Expression<Func<T, object>> orderByExpression = default,
        params Expression<Func<T, object>>[] includes)
        where T : class;
}