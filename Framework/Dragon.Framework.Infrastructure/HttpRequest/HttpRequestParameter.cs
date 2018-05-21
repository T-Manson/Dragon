using System.Collections.Generic;
using System.Text;

namespace Dragon.Framework.Infrastructure.HttpRequest
{
    /// <summary>
    /// HTTP请求参数
    /// </summary>
    public class HttpRequestParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpRequestParameter()
        {
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// 请求方式
        /// </summary>
        public RequestType RequestType { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// 请求Cookie对象
        /// </summary>
        public HttpCookieType Cookie { get; set; }

        /// <summary>
        /// 请求编码方式
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public IDictionary<string, string> Parameters { get; set; }
    }
}
