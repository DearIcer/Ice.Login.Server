using Ice.Login.Http.Common;
using Microsoft.Extensions.Caching.Memory;

namespace Ice.Login.Http.Middleware;

public class SessionValidationMiddleware
{
    private readonly IMemoryCache _cache;
    private readonly RequestDelegate _next;

    public SessionValidationMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!_cache.TryGetValue(context.Request.Cookies["userId"], out SessionModel session) ||
            session.ExpirationTime < DateTime.UtcNow)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}