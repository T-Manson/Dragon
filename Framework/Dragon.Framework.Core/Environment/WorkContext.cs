using System;
using Microsoft.AspNetCore.Http;

namespace Dragon.Framework.ApiCore.Common
{
    /// <summary>
    /// 请求上下文
    /// </summary>
    public static class WorkContext
    {
        private static IHttpContextAccessor _accessor;

        /// <summary>
        /// 当前请求上下文
        /// </summary>
        public static HttpContext Current => _accessor?.HttpContext;

        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            Console.WriteLine("上下文启用完成。");
        }
    }
}
