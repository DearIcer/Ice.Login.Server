using Ice.Login.Entity.Backend;
using Microsoft.EntityFrameworkCore;

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
