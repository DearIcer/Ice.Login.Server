using Ice.Login.Entity.Backend;
using Ice.Login.Repository.Context;
using Ice.Login.Repository.IRepository.ClientRepository.UserMng;
using Ice.Login.Repository.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ice.Login.Repository.Repository.ClientRepository.UserMng
{
    public class UserInfoRepository(IceDbContext dbContext) : DbRepository(dbContext), IUserInfoRepository
    {
        public async Task<bool> Create(UserInfo userInfo)
        {
            await DbContext.AddAsync(userInfo);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<UserInfo> Queryable(Expression<Func<UserInfo, bool>> whereExpression)
        {
            return await DbContext.UserInfo.AsQueryable().Where(whereExpression).FirstOrDefaultAsync();
        }
    }
}
