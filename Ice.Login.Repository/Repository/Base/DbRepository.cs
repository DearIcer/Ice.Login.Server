using Ice.Login.Repository.Context;
using Share;

namespace Ice.Login.Repository.Repository.Base
{
    public class DbRepository : BaseRepository, IUnitOfWork
    {
        protected IceDbContext _dbContext { get; }
        public DbRepository(IceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public void BeginTransaction()
        //{
        //    _dbContext.Database.BeginTransaction();
        //}

        //public void CommitTransaction()
        //{
        //    _dbContext.Database.CommitTransactionAsync;
        //}

        //public void RollbackTransaction()
        //{
        //    _dbContext.RollbackTransaction();   
        //}

        //public async Task<int> SaveChangesAsync()
        //{
        //    return await _dbContext.SaveChangesAsync();
        //}

        async Task IUnitOfWork.BeginTransaction()
        {
            await _dbContext.Database.BeginTransactionAsync();
        }

        async Task IUnitOfWork.CommitTransaction()
        {
            await _dbContext.Database.CommitTransactionAsync();
        }

        async Task IUnitOfWork.RollbackTransaction()
        {
            await _dbContext.Database.RollbackTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
