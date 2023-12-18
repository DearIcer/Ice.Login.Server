using Ice.Login.Entity.Backend;
using Ice.Login.Repository.IRepository.Base;
using Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ice.Login.Repository.IRepository.ClientRepository
{
    public interface IUserInfoRepository : IBaseRepository, IUnitOfWork
    {
        Task<UserInfo> Queryable(Expression<Func<UserInfo, bool>> whereExpression);
    }
}
