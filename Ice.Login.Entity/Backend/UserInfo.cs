namespace Ice.Login.Entity.Backend
{
    public class UserInfo : Base.Entity
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public string UserName { get; set; }

        public string Password { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

    }
}
