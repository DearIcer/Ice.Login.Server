namespace Ice.Login.Http.Middleware;

/// <summary>
///     请求中间件类，用于处理HTTP请求并获取客户端IP地址。
/// </summary>
public class RequestMiddleware
{
    private readonly ILogger<RequestMiddleware> _logger;
    private readonly RequestDelegate _next; // 下一个中间件或终端处理程序

    /// <summary>
    ///     构造函数。
    /// </summary>
    /// <param name="next">下一个中间件或终端处理程序的委托。</param>
    public RequestMiddleware(RequestDelegate next, ILogger<RequestMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    ///     处理HTTP请求的入口点。
    /// </summary>
    /// <param name="context">HTTP上下文，包含请求和响应的信息。</param>
    /// <returns>异步任务，表示请求处理的过程。</returns>
    public async Task Invoke(HttpContext context)
    {
        // 获取客户端IP地址
        var ipAddress = GetClientIpAddress(context);

        // 获取请求路径
        string requestPath = context.Request.Path;

        // 记录包含IP地址和请求路径的日志信息
        _logger.LogInformation($"Request received from IP: {ipAddress}, Path: {requestPath}");

        await _next(context); // 调用下一个中间件或处理程序
    }

    /// <summary>
    ///     获取客户端的IP地址。
    /// </summary>
    /// <param name="context">HTTP上下文。</param>
    /// <returns>客户端的IP地址，如果无法获取则返回"Unknown"。</returns>
    private static string GetClientIpAddress(HttpContext context)
    {
        // 尝试从转发的头部获取IP地址，适用于通过反向代理的情况
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedHeaders))
            return forwardedHeaders.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries)[0].Trim();

        // 如果没有转发的头部，则直接获取远程IP地址
        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}