using Ice.Login.Repository.Context;
using Share;

namespace Ice.Login.Repository.Repository.Base
{
    public class DbRepository : BaseRepository, IUnitOfWork
    {
        protected TestClass1 _dbContext { get; }
        public DbRepository(TestClass1 dbContext)
        {
            _dbContext = dbContext;
        }

        public void BeginTransaction()
        {
            _dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _dbContext.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            _dbContext.RollbackTransaction();   
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
