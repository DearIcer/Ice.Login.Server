namespace Common.Error;

public static class ErrorCode
{
    /// <summary>
    ///     未知错误
    /// </summary>
    public const string UnknownError = "99999";

    /// <summary>
    ///     账号错误
    /// </summary>
    public const string AccountError = "40000";

    /// <summary>
    ///     账号已存在
    /// </summary>
    public const string AccountExists = "40001";

    /// <summary>
    ///     密码错误
    /// </summary>
    public const string PasswordError = "40002";

    /// <summary>
    ///     令牌无效
    /// </summary>
    public const string TokenInvalid = "40003";

    /// <summary>
    ///     刷新令牌无效
    /// </summary>
    public const string RefreshTokenInvalid = "40004";

    /// <summary>
    ///     令牌过期
    /// </summary>
    public const string TokenExpired = "40005";

    /// <summary>
    ///     用户不存在
    /// </summary>
    public const string UserNotExists = "40006";
}

public class KnownException : Exception, IKnownException
{
    public static readonly IKnownException Unknown = new KnownException("unknown error", "99999");

    public KnownException(string message, string errorCode, params object[] errorData)
        : base(message)
    {
        ErrorCode = errorCode;
        ErrorData = errorData;
    }

    public KnownException(string message, Exception innerException, string errorCode, params object[] errorData)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        ErrorData = errorData;
    }

    public string ErrorCode { get; }

    public object[] ErrorData { get; }

    public static IKnownException FromKnownException(IKnownException exception)
    {
        return new KnownException(exception.Message, exception.ErrorCode, exception.ErrorData);
    }
}