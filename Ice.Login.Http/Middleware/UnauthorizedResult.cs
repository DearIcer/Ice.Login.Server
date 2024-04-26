using Ice.Login.Http.Common;
using Microsoft.Extensions.Caching.Memory;

namespace Ice.Login.Http.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public SessionValidationMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!_cache.TryGetValue(context.Request.Cookies["userId"], out SessionModel session) || session.ExpirationTime < DateTime.UtcNow)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
