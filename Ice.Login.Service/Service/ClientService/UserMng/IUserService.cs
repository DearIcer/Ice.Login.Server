using Ice.Login.Entity.Backend;
using Ice.Login.Service.Service.Base;

namespace Ice.Login.Service.Service.ClientService.UserMng;

public interface IUserService : IBaseService
{
    Task<UserInfo> Queryable();

    Task<bool> RegisterAccount(RegisterAccountRequest body);

    Task<LoginResponse> Login(LoginRequest body);
}