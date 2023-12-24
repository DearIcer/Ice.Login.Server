using System.Security.Cryptography;
using System.Text;

namespace Common.Utilities
{
    public static class HashTools
    {
        public static string MD5Encrypt32(string input = "")
        {
            string pwd = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input))
                {
                    MD5 md5 = MD5.Create();
                    byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                    foreach (var item in s)
                    {
                        pwd = string.Concat(pwd, item.ToString("X2"));
                    }
                }
            }
            catch
            {
                throw new Exception($"错误的 input 字符串:[{input}]");
            }
            return pwd;
        }
    }
}
