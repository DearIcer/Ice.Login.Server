using Ice.Login.Entity.Backend;
using Ice.Login.Repository.IRepository.ClientRepository;
using Ice.Login.Service.Service.Base;
using Share;
using System.Linq.Expressions;

namespace Ice.Login.Service.Service.ClientService.UserMng
{
    public class UserService : DbService, IUserService
    {
        private readonly IUserInfoRepository _userInfoRepository;

        public UserService(IUserInfoRepository userInfoRepository) : base(userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }

        public async Task<UserInfo> Queryable()
        {
            await BeginTransaction();
            var data = await _userInfoRepository.Queryable(it => it.Id >= 1);
            await CommitTransaction();
            return data;
        }
    }
}
