using System.Linq.Expressions;
using Ice.Login.Entity.Backend;
using Ice.Login.Repository.IRepository.Base;
using Share;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Ice.Login.Repository.IRepository.ClientRepository.UserMng;

public interface IUserInfoRepository : IBaseRepository, IUnitOfWork
{
    Task<bool> Create(UserInfo userInfo);

}