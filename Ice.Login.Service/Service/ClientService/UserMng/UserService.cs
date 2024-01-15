using Common.Error;
using Common.Utilities;
using Ice.Login.Entity.Backend;
using Ice.Login.Repository.IRepository.ClientRepository.UserMng;
using Ice.Login.Service.Service.Base;
using Share;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

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
            UserInfo data = await _userInfoRepository.Queryable(it => it.UserName == body.UserName);
            if (data != null)
            {
                throw new KnownException("用户名已存在", ErrorCode.AccountExists);
            }
            //await RegisterParameterCalibration(body.NickName, body.UserName, body.Password);
            //await PassworldCalibration(body.Password);
            UserInfo user = new UserInfo()
            {
                NickName = body.NickName,
                UserName = body.UserName,
                Password = HashTools.MD5Encrypt32(body.Password),
                IsDelete = false,
            };
            await BeginTransaction();
            var reuslt = await _userInfoRepository.Create(user);
            await CommitTransaction();
            return reuslt;
        }
        private async Task<bool> RegisterParameterCalibration(string nickName, string userName, string password)
        {
            if (string.IsNullOrEmpty(nickName) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new KnownException("昵称、用户名或密码不能为空", ErrorCode.AccountError);
            }

            if (nickName.Length > 12 || password.Length > 16 || userName.Length > 16)
            {
                string errorMessage = string.Empty;
                if (nickName.Length > 12)
                {
                    errorMessage += "昵称过长. ";
                }
                if (password.Length > 16)
                {
                    errorMessage += "密码过长. ";
                }
                if (password.Length < 6)
                {
                    errorMessage += "密码过短";
                }
                if (userName.Length > 16)
                {
                    errorMessage += "用户名过长. ";
                }

                throw new KnownException(errorMessage.Trim(), "10001");
            }

            return true;
        }
        private async Task<bool> PassworldCalibration(string password)
        {
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_])[A-Za-z\d@$!%*?&_]{8,}$");
            if (!regex.IsMatch(password))
            {
                throw new KnownException("密码至少包含8个字符，包括至少一个大写字母、一个小写字母、一个数字和一个特殊字符", "10001");
            }
            return true;

        }
        public async Task<LoginResponse> Login(LoginRequest body)
        {
            UserInfo userInfo = await _userInfoRepository.Queryable(it => it.UserName == body.UserName &&
            it.Password == HashTools.MD5Encrypt32(body.Password));   
            if(userInfo == null)
            {
                throw new KnownException("用户名或密码错误", ErrorCode.PasswordError);
            }
            return new LoginResponse() { UserName = userInfo.UserName,Token = Guid.NewGuid().ToString()};
        }
    }
}
