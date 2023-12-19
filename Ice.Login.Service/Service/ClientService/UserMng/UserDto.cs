namespace Ice.Login.Service.Service.ClientService.UserMng
{
    /// <summary>
    /// 注册账号请求体
    /// </summary>
    public class RegisterAccountRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>

        public string Password { get; set; }
    }
}
