namespace Common.Error
{
    public interface IKnownException
    {
        public string Message { get; }

        public string ErrorCode { get; }

        public object[] ErrorData { get; }
    }
}
