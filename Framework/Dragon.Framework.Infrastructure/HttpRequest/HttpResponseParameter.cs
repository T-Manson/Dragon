using System;
using System.Net;

namespace Dragon.Framework.Infrastructure.HttpRequest
{
    /// <summary>
    /// 响应参数
    /// </summary>
    public class HttpResponseParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpResponseParameter()
        {
            Cookie = new HttpCookieType();
        }

        /// <summary>
        /// 响应资源标识符
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// 响应状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 响应Cookie对象
        /// </summary>
        public HttpCookieType Cookie { get; set; }

        /// <summary>
        /// 响应体
        /// </summary>
        public string Body { get; set; }
    }
}
