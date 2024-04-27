namespace Common.Model;

public class ApiResult
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public bool Success { get; set; } = true;

    public string ErrorCode { get; set; }

    public string Message { get; set; }

    public object Data { get; set; }

    public string Timestamp { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
}