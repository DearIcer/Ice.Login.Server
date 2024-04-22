using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common.Error;
using Common.Utilities;
using Ice.Login.Entity.Backend;
using Ice.Login.Repository.IRepository.ClientRepository.UserMng;
using Ice.Login.Service.Common.JWT;
using Ice.Login.Service.Service.Base;
using Microsoft.IdentityModel.Tokens;

namespace Ice.Login.Service.Service.ClientService.UserMng;

public class UserService(IUserInfoRepository userInfoRepository) : DbService(userInfoRepository), IUserService
{
    public async Task<UserInfo> Queryable()
    {
        var data = await userInfoRepository.Queryable(it => it.Id >= 1);
        return data;
    }

    public async Task<bool> RegisterAccount(RegisterAccountRequest body)
    {
        var data = await userInfoRepository.Queryable(it => it.UserName == body.UserName);
        if (data != null) throw new KnownException("用户名已存在", ErrorCode.AccountExists);
        //await RegisterParameterCalibration(body.NickName, body.UserName, body.Password);
        //await PassworldCalibration(body.Password);
        var user = new UserInfo
        {
            NickName = body.NickName,
            UserName = body.UserName,
            Password = HashTools.MD5Encrypt32(body.Password),
            IsDelete = false
        };
        await BeginTransaction();
        var reuslt = await userInfoRepository.Create(user);
        await CommitTransaction();
        return reuslt;
    }

    public async Task<LoginResponse> Login(LoginRequest body)
    {
        var userInfo = await userInfoRepository.Queryable(it => it.UserName == body.UserName &&
                                                                 it.Password == HashTools.MD5Encrypt32(body.Password));
        if (userInfo == null) throw new KnownException("用户名或密码错误", ErrorCode.PasswordError);


        var token = JwtTokenGenerator.GenerateToken(userInfo);
        return new LoginResponse { UserName = userInfo.UserName, accessToken = token.Token ,RefreshToken = token.RefreshToken};
    }
}