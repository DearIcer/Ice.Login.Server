using System.Linq.Expressions;
using Ice.Login.Entity.Backend;
using Ice.Login.Repository.IRepository.Base;
using Share;

namespace Ice.Login.Repository.IRepository.ClientRepository.UserMng;

public interface IUserInfoRepository : IBaseRepository, IUnitOfWork
{
    Task<UserInfo> GetUserinfo(Expression<Func<UserInfo, bool>> whereExpression);

    Task<bool> Create(UserInfo userInfo);
}