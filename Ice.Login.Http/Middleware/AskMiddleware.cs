using System.Text.RegularExpressions;

namespace Ice.Login.Http.Middleware;

public class AskMiddleware
{
    /// <summary>
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    ///     日志
    /// </summary>
    private readonly ILogger<AskMiddleware> _logger;

    private bool _isRegisted;


    /// <summary>
    /// </summary>
    /// <param name="next"></param>
    public AskMiddleware(RequestDelegate next, ILogger<AskMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _logger.BeginScope("ScopeId:{ScopeId}", Guid.NewGuid());
    }


    /// <summary>
    ///     释放
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.Now;

        if (context.Request.Path.Value!.Contains("api") || context.Request.Path.Value!.Contains("ws"))
        {
            context.Request.EnableBuffering();
            var olnycode = context.TraceIdentifier;
            var originalBody = context.Response.Body;

            await RequestDataLog(context, olnycode);

            using (var ms = new MemoryStream())
            {
                //存储响应数据
                context.Response.Body = ms;
                await _next(context);
                ResponseDataLog(ms, olnycode);
                ms.Position = 0;
                await ms.CopyToAsync(originalBody);
            }

            var endTime = DateTime.Now;
            var span = endTime - startTime;
            if (span.TotalSeconds > 1)
            {
                _logger.LogInformation("ASK-" + olnycode + "请求耗时" + span.TotalSeconds + "秒");
            }
        }
        else
        {
            await _next(context);
        }
    }

    private async Task RequestDataLog(HttpContext context, string code)
    {
        var request = context.Request;

        var sr = new StreamReader(request.Body);
        var content = $" QueryData:{request.Path + request.QueryString}\r\n BodyData:{await sr.ReadToEndAsync()}";
        if (!string.IsNullOrEmpty(content))
        {
            Parallel.For(0, 1, _ => { _logger.LogInformation("ASK-" + code + "输入数据\r\n" + content); });
            request.Body.Position = 0;
        }
    }

    private void ResponseDataLog(MemoryStream ms, string code)
    {
        ms.Position = 0;
        var responseBody = new StreamReader(ms).ReadToEnd();

        // 去除 Html
        var reg = "<[^>]+>";
  
        if (!string.IsNullOrEmpty(responseBody))
            Parallel.For(0, 1, _ => { _logger.LogInformation("ASK-" + code + "输出数据\r\n" + Regex.Unescape(responseBody)); });
    }
}