using System.Linq.Expressions;
using Common.Error;
using Common.Model;
using Common.Utilities;
using Ice.Login.Entity.Backend;
using Ice.Login.Repository.IRepository.ClientRepository.UserMng;
using Ice.Login.Service.Common.JWT;
using Ice.Login.Service.Service.Base;
using Microsoft.Extensions.Caching.Memory;

namespace Ice.Login.Service.Service.ClientService.UserMng;

public class UserService(
    IUserInfoRepository userInfoRepository,
    IMemoryCache cache,
    IJwtTokenGenerator jwtTokenGenerator)
    : DbService(userInfoRepository), IUserService
{
    public async Task<UserInfo> Queryable()
    {
        var data = await userInfoRepository.Queryable<UserInfo>(it => it.Id >= 1);
        return data;
    }

    public async Task<bool> RegisterAccount(RegisterAccountRequest body)
    {
        var data = await userInfoRepository.Queryable<UserInfo>(it => it.UserName == body.UserName);
        if (data != null) throw new KnownException("用户名已存在", ErrorCode.AccountExists);

        var user = new UserInfo
        {
            NickName = body.NickName,
            UserName = body.UserName,
            Password = HashTools.Md5Encrypt32(body.Password),
            IsDelete = false
        };
        await BeginTransaction();
        var result = await userInfoRepository.Create(user);
        await CommitTransaction();
        return result;
    }

    public async Task<LoginResponse> Login(LoginRequest body)
    {
        var userInfo = await userInfoRepository.Queryable<UserInfo>(it => it.UserName == body.UserName &&
                                                                          it.Password == HashTools.Md5Encrypt32(body.Password));
        if (userInfo == null) throw new KnownException("用户名或密码错误", ErrorCode.PasswordError);

        var token = jwtTokenGenerator.GenerateToken(userInfo);
        var sessionModel = new SessionModel
        {
            UserId = userInfo.Id,
            ExpirationTime = DateTime.UtcNow.AddMinutes(60)
        };
        cache.Set(userInfo.Id, sessionModel, TimeSpan.FromMinutes(60));
        return new LoginResponse
            { UserName = userInfo.UserName, accessToken = token.Token, RefreshToken = token.RefreshToken };
    }

    public async Task<LoginResponse> RefreshToken(string refreshToken)
    {
        long userId = 0;
        if (!cache.TryGetValue(refreshToken, out userId))
            throw new KnownException("refreshToken无效", ErrorCode.RefreshTokenInvalid);

        var userInfo = await userInfoRepository.Queryable<UserInfo>(info => info.Id == userId);
        if (userInfo == null) throw new KnownException("用户不存在", ErrorCode.UserNotExists);
        cache.Remove(refreshToken);
        var token = jwtTokenGenerator.GenerateToken(userInfo);
        var sessionModel = new SessionModel
        {
            UserId = userInfo.Id,
            ExpirationTime = DateTime.UtcNow.AddMinutes(60)
        };
        cache.Set(userInfo.Id, sessionModel, TimeSpan.FromMinutes(60));
        return new LoginResponse
            { UserName = userInfo.UserName, accessToken = token.Token, RefreshToken = token.RefreshToken };
    }

    public async Task<(int count, List<UserInfo>)> QueryableList(int pageIndex, int pageSize, string query)
    {
        var data = await userInfoRepository.GetPagedDataWithFilterAsync(it => true, GetWhereExpression(query),
            pageIndex, pageSize);
        return (data.TotalCount, data.Data);
    }
    
    private Expression<Func<UserInfo, bool>> GetWhereExpression(string query)
    {
        if (string.IsNullOrEmpty(query)) return userInfo => true;
        return userInfo => userInfo.NickName.Contains(query) || userInfo.UserName.Contains(query);
    }
}