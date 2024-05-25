using Ice.Login.Entity.Backend;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Ice.Login.Repository.Context;

public class IceDbContext(DbContextOptions<IceDbContext> options) : DbContext(options)
{
    public DbSet<UserInfo> UserInfo { get; set; }
}