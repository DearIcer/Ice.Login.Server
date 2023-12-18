using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Share
{
    public interface IUnitOfWork
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        Task<int> SaveChangesAsync();
    }
    public class UnitOfWork : DbContext, IUnitOfWork
    {
        private readonly DbContext _dbContext;
        private IDbContextTransaction _transaction;

        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual void BeginTransaction()
        {
            _transaction = _dbContext.Database.BeginTransaction();
        }

        public virtual void CommitTransaction()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        public virtual void RollbackTransaction()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        public async virtual Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }

}
