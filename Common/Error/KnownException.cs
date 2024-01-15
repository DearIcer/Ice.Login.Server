namespace Common.Error
{
    public static class ErrorCode
    {
        public const string UnknownError = "99999";
        public const string AccountError = "40000";
        public const string AccountExists = "40001";
        public const string PasswordError = "40002";    
    }
    public class KnownException : Exception, IKnownException
    {
        public readonly static IKnownException Unknown = new KnownException("unknown error", "99999");
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

        public string ErrorCode { get; private set; }

        public object[] ErrorData { get; private set; }

        public static IKnownException FromKnownException(IKnownException exception)
        {
            return new KnownException(exception.Message, exception.ErrorCode, exception.ErrorData);
        }
    }
}
