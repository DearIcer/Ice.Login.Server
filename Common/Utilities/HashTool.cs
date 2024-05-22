using System.Security.Cryptography;
using System.Text;

namespace Common.Utilities;

public static class HashTools
{
    public static string Md5Encrypt32(string input = "")
    {
        var pwd = string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input))
            {
                var md5 = MD5.Create();
                var s = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                foreach (var item in s) pwd = string.Concat(pwd, item.ToString("X2"));
            }
        }
        catch
        {
            throw new Exception($"错误的 input 字符串:[{input}]");
        }

        return pwd;
    }
}