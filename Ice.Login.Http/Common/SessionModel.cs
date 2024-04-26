namespace Ice.Login.Http.Common;

public class SessionModel
{
    public string UserId { get; set; }
    public DateTime ExpirationTime { get; set; }
}