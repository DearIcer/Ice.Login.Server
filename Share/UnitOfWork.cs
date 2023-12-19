using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Share
{
    public interface IUnitOfWork
    {
        Task BeginTransaction();
        Task CommitTransaction();
        Task RollbackTransaction();
        Task<int> SaveChangesAsync();
    }
    public class UnitOfWork : DbContext, IUnitOfWork
    {
        private IDbContextTransaction _transaction;
        public async virtual Task BeginTransaction()
        {
            _transaction = await Database.BeginTransactionAsync();
        }

        public async virtual Task CommitTransaction()
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async virtual Task RollbackTransaction()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async virtual Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync();
        }
    }

}
