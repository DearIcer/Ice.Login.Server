using Ice.Login.Entity.Backend;
using Ice.Login.Repository.Context;
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
     
}