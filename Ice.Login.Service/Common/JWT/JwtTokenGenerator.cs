using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ice.Login.Entity.Backend;
using Microsoft.IdentityModel.Tokens;

namespace Ice.Login.Service.Common.JWT;

public static class JwtTokenGenerator
{
    private static readonly string SecretKey = "B7FC4838F5A36C8D1940B56FE2DF734293434DA17CCA35A4E3F82D66EAD6AA0D";

    public static string GenerateToken(string userId, string userName, string secretKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Name, userId),
                new(ClaimTypes.NameIdentifier, userName)
            }),
            Expires = DateTime.UtcNow.AddHours(1)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static TokenResult GenerateToken(UserInfo userInfo)
    {
        // 生成时间戳
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // 结合时间戳和用户名
        var payload = $"{timestamp}:{userInfo.UserName}";

        // 创建JWT令牌
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Name, userInfo.UserName),
                // 添加其他必要声明
                new("timestamp", timestamp.ToString()) // 如果需要，添加时间戳声明
            }),
            Expires = DateTime.UtcNow.AddHours(1), // 设置令牌过期时间
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
                SecurityAlgorithms.HmacSha256Signature)
        };

        // 计算签名
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var signature = Convert.ToBase64String(signatureBytes);

        // 将签名添加到声明中
        tokenDescriptor.Subject.AddClaim(new Claim("signature", signature));

        // 创建令牌
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // **生成刷新令牌**
        // 1. 为刷新令牌定义一个唯一标识符
        var refreshTokenId = Guid.NewGuid();

        // 2. 生成一个安全随机字符串作为刷新令牌的实际值
        var refreshTokenValue = GenerateSecureRandomString(32); // 实现GenerateSecureRandomString()方法

        // 3. 可选地，为刷新令牌设置过期时间（例如，7天）
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

        // **将令牌存储在数据库或缓存中以供后续使用（如刷新令牌、令牌撤销）**
        // 您可以在这里添加自己的逻辑来存储和管理令牌
        // 例如：
        // - 将刷新令牌详细信息（refreshTokenId, refreshTokenValue, refreshTokenExpiration, userInfo.UserId）保存到数据库中
        // - 将生成的JWT令牌与refreshTokenId关联起来，以便在令牌验证期间轻松查找

        var tokenResult = new TokenResult
        {
            Token = tokenString,
            RefreshToken = refreshTokenValue
        };
        return tokenResult;
    }

    private static string GenerateSecureRandomString(int i)
    {
        var randomNumber = new byte[i];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

/// <summary>
///     令牌结果
/// </summary>
public class TokenResult
{
    /// <summary>
    ///     令牌
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    ///     刷新令牌
    /// </summary>
    public string RefreshToken { get; set; }
}