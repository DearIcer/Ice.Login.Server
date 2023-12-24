using System.ComponentModel.DataAnnotations;

namespace Ice.Login.Service.Service.ClientService.UserMng
{
    /// <summary>
    /// 注册账号请求体
    /// </summary>
    public class RegisterAccountRequest
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [Required]
        public string NickName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>

        [Required]
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }          
        
        /// <summary>
        /// token
        /// </summary>
        [Required]
        public string Token { get; set; }
      
    }
}
