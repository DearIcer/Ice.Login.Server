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
}