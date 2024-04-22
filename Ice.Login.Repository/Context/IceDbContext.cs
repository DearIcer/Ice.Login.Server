using Ice.Login.Entity.Backend;
using Microsoft.EntityFrameworkCore;

namespace Ice.Login.Repository.Context;

public class IceDbContext(DbContextOptions<IceDbContext> options) : DbContext(options)
{
    public DbSet<UserInfo> UserInfo { get; set; }
}