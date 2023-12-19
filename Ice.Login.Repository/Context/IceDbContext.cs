using Ice.Login.Entity.Backend;
using Microsoft.EntityFrameworkCore;
using Share;

namespace Ice.Login.Repository.Context
{
    public class IceDbContext : DbContext
    {
        public IceDbContext(DbContextOptions<IceDbContext> options) : base(options)
        {
        }
        public DbSet<UserInfo> UserInfo { get; set; }
    }
}
