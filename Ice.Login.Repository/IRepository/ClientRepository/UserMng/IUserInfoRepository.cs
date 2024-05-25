using System.Linq.Expressions;
using Ice.Login.Entity.Backend;
using Ice.Login.Repository.IRepository.Base;
using Share;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Ice.Login.Repository.IRepository.ClientRepository.UserMng;

public interface IUserInfoRepository : IBaseRepository, IUnitOfWork
{
    Task<UserInfo> GetUserinfo(Expression<Func<UserInfo, bool>> whereExpression);

    Task<bool> Create(UserInfo userInfo);

    Task<(int count, List<UserInfo>)> QueryableList(Expression<Func<UserInfo, bool>> whereExpression, int pageIndex,
        int pageSize, string query);
}