using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Model;
using Ice.Login.Service.Common.JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Ice.Login.Http.Filter;

public class SessionValidationFilter(
    IMemoryCache cache,
    JwtTokenConfig jwtTokenConfig,
    ILogger<SessionValidationFilter> logger) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // 放行swagger
        if (context.HttpContext.Request.Path.StartsWithSegments("/swagger") ||
            context.HttpContext.Request.Path.StartsWithSegments("/swagger-ui") ||
            context.HttpContext.Request.Path.StartsWithSegments("/swagger/v1/swagger.json"))
            return;

        // 从请求头 "Authorization" 中提取 JWT 令牌
        var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader) ||
            !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // 提取 JWT 令牌（去除 "Bearer " 前缀）
        var jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();

        try
        {
            // 验证并解析 JWT 令牌
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtTokenConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtTokenConfig.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfig.Secret)),
                ClockSkew = TimeSpan.Zero // 严格校验过期时间，无时间偏移
            };

            var claimsPrincipal =
                new JwtSecurityTokenHandler().ValidateToken(jwtToken, tokenValidationParameters,
                    out var validatedToken);

            // 验证成功，从 JWT 令牌的 Claims 中获取用户 ID
            var userId = Convert.ToInt64(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

            SessionModel session = new();
            if (!cache.TryGetValue(userId, out session) ||
                session.ExpirationTime < DateTime.UtcNow)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
        catch (SecurityTokenException ex)
        {
            // 验证失败，返回 401 Unauthorized
            logger.LogError(ex, "JWT token validation failed.");
            context.Result = new UnauthorizedResult();
            return;
        }

        base.OnActionExecuting(context);
    }
}