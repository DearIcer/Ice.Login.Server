using System.ComponentModel.DataAnnotations;

namespace Ice.Login.Service.Service.ClientService.UserMng;

/// <summary>
///     注册账号请求体
/// </summary>
public class RegisterAccountRequest
{
    /// <summary>
    ///     昵称
    /// </summary>
    [Required(ErrorMessage = "昵称不能为空!")]
    [StringLength(12, ErrorMessage = "昵称过长,长度不能超过6个字符!")]
    public string NickName { get; set; }

    /// <summary>
    ///     用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空!")]
    [StringLength(16, ErrorMessage = "用户名过长,长度不能超过16个字符!")]
    public string UserName { get; set; }

    /// <summary>
    ///     密码
    /// </summary>

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_])[A-Za-z\d@$!%*?&_]{8,}$",
        ErrorMessage = "密码至少包含8个字符，包括至少一个大写字母、一个小写字母、一个数字和一个特殊字符.")]
    public string Password { get; set; }
}

public class LoginRequest
{
    /// <summary>
    ///     用户名
    /// </summary>
    [Required(ErrorMessage = "昵称不能为空!")]
    public string UserName { get; set; }

    /// <summary>
    ///     密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空!")]
    public string Password { get; set; }
}

public class LoginResponse
{
    /// <summary>
    ///     用户名
    /// </summary>
    [Required]
    public string UserName { get; set; }

    /// <summary>
    ///     token
    /// </summary>
    [Required]
    public string accessToken { get; set; }
    
    /// <summary>
    ///     刷新token
    /// </summary>
    public string RefreshToken { get; set; }
}