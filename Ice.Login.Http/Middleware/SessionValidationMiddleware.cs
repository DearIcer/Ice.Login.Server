using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Model;
using Ice.Login.Service.Common.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace Ice.Login.Http.Middleware;

public class SessionValidationMiddleware(
    RequestDelegate next,
    IMemoryCache cache,
    JwtTokenConfig jwtTokenConfig,
    ILogger<SessionValidationMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await next(context);
            return;
        }

        // 放行swagger
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/swagger-ui") ||
            context.Request.Path.StartsWithSegments("/swagger/v1/swagger.json"))
        {
            await next(context);
            return;
        }

        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader) ||
            !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers[HeaderNames.WWWAuthenticate] = "Bearer error=\"invalid_token\"";
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

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

            var handler = new JwtSecurityTokenHandler();
            var claimsPrincipal = handler.ValidateToken(jwtToken, tokenValidationParameters, out var validatedToken);

            var userId = Convert.ToInt64(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

            SessionModel session;
            if (!cache.TryGetValue(userId, out session) ||
                session.ExpirationTime < DateTime.UtcNow)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Headers[HeaderNames.WWWAuthenticate] = "Bearer error=\"session_expired\"";
                await context.Response.WriteAsync("Unauthorized - Session Expired");
                return;
            }
        }
        catch (SecurityTokenException ex)
        {
            logger.LogError(ex, "JWT token validation failed.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers[HeaderNames.WWWAuthenticate] = "Bearer error=\"token_invalid\"";
            await context.Response.WriteAsync("Unauthorized - Token Invalid");
            return;
        }

        await next(context);
    }
}