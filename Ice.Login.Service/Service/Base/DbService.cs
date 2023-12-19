using Ice.Login.Service.IService.Base;
using Share;

namespace Ice.Login.Service.Service.Base
{
    public class DbService : BaseService, IDbService
    {
        protected IUnitOfWork _unitOfWork { get; init; }
        public DbService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public async Task BeginTransaction()
        {
            await _unitOfWork.BeginTransaction();
        }

        public async Task CommitTransaction()
        {
            await _unitOfWork.CommitTransaction();  
        }

        public async Task RollbackTransaction()
        {
            await _unitOfWork.RollbackTransaction();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
