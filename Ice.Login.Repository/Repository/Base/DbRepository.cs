using Ice.Login.Repository.Context;
using Share;

namespace Ice.Login.Repository.Repository.Base;

public class DbRepository(IceDbContext dbContext) : BaseRepository, IUnitOfWork
{
    protected IceDbContext DbContext { get; } = dbContext;

    async Task IUnitOfWork.BeginTransaction()
    {
        await DbContext.Database.BeginTransactionAsync();
    }

    async Task IUnitOfWork.CommitTransaction()
    {
        await DbContext.Database.CommitTransactionAsync();
    }

    async Task IUnitOfWork.RollbackTransaction()
    {
        await DbContext.Database.RollbackTransactionAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }
}