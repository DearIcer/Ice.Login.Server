using Ice.Login.Entity.Backend;
using Ice.Login.Repository.Context;
using Ice.Login.Repository.IRepository.ClientRepository;
using Ice.Login.Repository.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ice.Login.Repository.Repository.ClientRepository
{
    public class UserInfoRepository : DbRepository, IUserInfoRepository
    {
        public UserInfoRepository(TestClass1 dbContext) : base(dbContext)
        {
        }

        public async Task<UserInfo> Queryable(Expression<Func<UserInfo, bool>> whereExpression)
        {
            return await _dbContext.UserInfo.AsQueryable().Where(whereExpression).FirstAsync();
        }
    }
}
