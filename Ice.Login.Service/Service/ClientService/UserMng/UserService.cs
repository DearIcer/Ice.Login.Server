using Ice.Login.Entity.Backend;
using Ice.Login.Repository.IRepository.ClientRepository.UserMng;
using Ice.Login.Service.Service.Base;
using Share;
using System.Linq.Expressions;

namespace Ice.Login.Service.Service.ClientService.UserMng
{
    public class UserService(IUserInfoRepository userInfoRepository) : DbService(userInfoRepository), IUserService
    {
        private readonly IUserInfoRepository _userInfoRepository = userInfoRepository;

        public async Task<UserInfo> Queryable()
        {
            var data = await _userInfoRepository.Queryable(it => it.Id >= 1);
            return data;
        }

        public async Task<bool> RegisterAccount(RegisterAccountRequest body)
        {
            var data = await _userInfoRepository.Queryable(it => it.UserName == body.UserName);
            if (data != null)
            {
                return false;
            }
            UserInfo user = new UserInfo()
            {
                UserName = body.UserName,
                Password = body.Password,
                IsDelete = false,
            };
            await BeginTransaction();
            var reuslt = await _userInfoRepository.Create(user);
            await CommitTransaction();
            return reuslt;
        }
    }
}
