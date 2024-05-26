using System.Collections.Concurrent;

namespace Ice.Login.Http.Middleware;
public class RequestPeakMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestPeakMiddleware> _logger;
    private readonly ConcurrentDictionary<DateTime, int> _requestCounts;
    private readonly TimeSpan _peakPeriod;
    private int _peakValue;

    public RequestPeakMiddleware(RequestDelegate next, ILogger<RequestPeakMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _requestCounts = new ConcurrentDictionary<DateTime, int>();
        _peakPeriod = TimeSpan.FromMinutes(1); // 设置峰值统计的时间段为一分钟
        _peakValue = 0;

        // 启动一个定时任务，每分钟检查一次当前请求量是否是峰值
        Task.Run(() => UpdatePeakValue());
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var currentTime = DateTime.UtcNow;
        _requestCounts.AddOrUpdate(currentTime, 1, (key, value) => value + 1);

        await _next(context);
    }

    private void UpdatePeakValue()
    {
        while (true)
        {
            var currentTime = DateTime.UtcNow;
            var startTime = currentTime.Subtract(_peakPeriod);

            // 移除过期的记录
            foreach (var key in _requestCounts.Keys)
            {
                if (key < startTime)
                {
                    _requestCounts.TryRemove(key, out _);
                }
            }

            // 统计当前时间段内的请求数
            var count = 0;
            foreach (var value in _requestCounts.Values)
            {
                count += value;
            }

            // 更新峰值
            _peakValue = count > 0 ? Math.Max(_peakValue, count) : 0; // 如果count为0，则将峰值设为0

            // 输出当前峰值
            _logger.LogInformation($"Peak value: {_peakValue}");

            Thread.Sleep(6000); // 每分钟检查一次
        }
    }

}

public static class RequestPeakMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestPeakMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestPeakMiddleware>();
    }
}