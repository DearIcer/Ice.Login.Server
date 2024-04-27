using Ice.Login.Http.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace Ice.Login.Http.Filter;

public class SessionValidationFilter : ActionFilterAttribute
{
    private readonly IMemoryCache _cache;

    public SessionValidationFilter(IMemoryCache cache)
    {
        _cache = cache;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // 在此处从请求中提取用户ID（实际逻辑取决于您的会话管理方式）

        if (!_cache.TryGetValue(context.HttpContext.Request.Cookies["userId"], out SessionModel session) ||
            session.ExpirationTime < DateTime.UtcNow)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        base.OnActionExecuting(context);
    }
}