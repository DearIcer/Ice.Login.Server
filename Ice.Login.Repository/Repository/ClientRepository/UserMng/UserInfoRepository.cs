using System.Linq.Expressions;
using Ice.Login.Entity.Backend;
using Ice.Login.Repository.Context;
using Ice.Login.Repository.Extensions;
using Ice.Login.Repository.IRepository.ClientRepository.UserMng;
using Ice.Login.Repository.Repository.Base;
using Microsoft.Extensions.Logging;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Ice.Login.Repository.Repository.ClientRepository.UserMng;

public class UserInfoRepository(IceDbContext dbContext, ILogger<DbRepository> logger)
    : DbRepository(dbContext, logger), IUserInfoRepository
{
    public async Task<bool> Create(UserInfo userInfo)
    {
        await DbContext.AddAsync(userInfo);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<(int count, List<UserInfo>)> QueryableList(Expression<Func<UserInfo, bool>> whereExpression,
        int pageIndex, int pageSize, string query)
    {
        var includes = new Expression<Func<UserInfo, object>>[] { };
        var data = await DbContext.UserInfo.GetListToPageAsync(whereExpression, pageIndex, pageSize,
            GetWhereExpression(query), it => it.Id, includes);
        return (data.Total, data.List);
    }

    public async Task<UserInfo> GetUserinfo(Expression<Func<UserInfo, bool>> whereExpression)
    {
        return await DbContext.UserInfo.GetAsync(whereExpression);
        // return await Queryable(whereExpression);
        // return await DbContext.UserInfo.AsQueryable().Where(whereExpression).FirstOrDefaultAsync();
    }

    private Expression<Func<UserInfo, bool>> GetWhereExpression(string query)
    {
        if (string.IsNullOrEmpty(query)) return userInfo => true;
        return userInfo => userInfo.NickName.Contains(query) || userInfo.UserName.Contains(query);
    }
}