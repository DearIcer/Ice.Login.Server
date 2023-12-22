using System.ComponentModel.DataAnnotations;

namespace Ice.Login.Entity.Backend
{
    public class UserInfo : Base.Entity
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        [Required]
        public string NickName { get; set; }    

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Required]
        public bool IsDelete { get; set; } 

        public bool BanAccount { get; set; }

        public DateTime? BanPeriod { get; set; }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。


    }
}
