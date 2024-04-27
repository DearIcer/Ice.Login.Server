using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ice.Login.Entity.Backend;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Ice.Login.Service.Common.JWT;

public class JwtTokenGenerator(JwtTokenConfig jwtTokenConfig, IMemoryCache cache) : IJwtTokenGenerator
{
    private readonly string _secretKey = jwtTokenConfig.Secret;

    public string GenerateToken(string userId, string userName, string secretKey)
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

    public TokenResult GenerateToken(UserInfo userInfo)
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
                new("timestamp", timestamp.ToString()),
                new(ClaimTypes.NameIdentifier, userInfo.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1), // 设置令牌过期时间
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                SecurityAlgorithms.HmacSha256Signature),
            Audience = jwtTokenConfig.Audience,
            Issuer = jwtTokenConfig.Issuer
        };

        // 计算签名
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var signature = Convert.ToBase64String(signatureBytes);

        // 将签名添加到声明中
        tokenDescriptor.Subject.AddClaim(new Claim("signature", signature));

        // 创建令牌
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // **生成刷新令牌**
        // 1. 生成一个安全随机字符串作为刷新令牌的实际值
        var refreshTokenValue = GenerateSecureRandomString(32);

        // 2. 可选地，为刷新令牌设置过期时间（例如，2小时）
        var refreshTokenExpiration = TimeSpan.FromMinutes(55);

        // **将令牌存储在数据库或缓存中以供后续使用（如刷新令牌、令牌撤销）**
        // 例如：
        // - 将令牌存储到缓存
        cache.Set(refreshTokenValue, userInfo.Id, refreshTokenExpiration);

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