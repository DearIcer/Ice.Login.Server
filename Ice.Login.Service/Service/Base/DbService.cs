using Share;

namespace Ice.Login.Service.Service.Base;

public class DbService(IUnitOfWork unitOfWork) : BaseService, IDbService
{
    protected IUnitOfWork UnitOfWork { get; init; } = unitOfWork;

    public async Task BeginTransaction()
    {
        await UnitOfWork.BeginTransaction();
    }

    public async Task CommitTransaction()
    {
        await UnitOfWork.CommitTransaction();
    }

    public async Task RollbackTransaction()
    {
        await UnitOfWork.RollbackTransaction();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await UnitOfWork.SaveChangesAsync();
    }
}