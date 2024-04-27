namespace Common.Model;

public class SessionModel
{
    public long UserId { get; set; }
    public DateTime ExpirationTime { get; set; }
}