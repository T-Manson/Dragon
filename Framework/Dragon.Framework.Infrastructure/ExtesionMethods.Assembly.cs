using System.Reflection;
using System.Text;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static partial class ExtensionMethods
    {
        /// <summary>
        /// 获取程序集公钥字符串。
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetPublicKeyString(this Assembly assembly)
        {
            var bytes = assembly.GetName().GetPublicKey();
            if (bytes.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append($"{b:x}");
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// 获取公钥标记字符串，该标记为应用程序或程序集签名时所用公钥的 SHA-1 哈希值的最后 8 个字节。
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetPublicKeyTokenString(this Assembly assembly)
        {
            var bytes = assembly.GetName().GetPublicKeyToken();
            if (bytes.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append($"{b:x}");
            }
            return sb.ToString().Trim();
        }
    }
}
