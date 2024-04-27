using Ice.Login.Entity.Backend;

namespace Ice.Login.Service.Common.JWT;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string userName, string secretKey);
    TokenResult GenerateToken(UserInfo userInfo);
}