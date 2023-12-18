using Ice.Login.Entity.Backend;
using Microsoft.EntityFrameworkCore;
using Share;

namespace Ice.Login.Repository.Context
{
    public class TestClass1 : UnitOfWork
    {
        public TestClass1(DbContext dbContext) : base(dbContext)
        {
        }
        public DbSet<UserInfo> UserInfo { get; set; }
    }
}
