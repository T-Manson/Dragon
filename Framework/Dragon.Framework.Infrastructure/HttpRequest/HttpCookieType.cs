using System.Net;

namespace Dragon.Framework.Infrastructure.HttpRequest
{
    /// <summary>
    /// Cookie类型
    /// </summary>
    public class HttpCookieType
    {
        /// <summary>
        /// cookie集合
        /// </summary>
        public CookieCollection Cookies { get; set; }

        /// <summary>
        /// 单个cookie值
        /// </summary>
        public string Cookie { get; set; }
    }
}
