using Dragon.Framework.ApiCore.Common;
using Dragon.Framework.Core.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dragon.Framework.ApiCore.Extensions
{
    /// <summary>
    /// ApplicationBuilder扩展
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 启用静态请求上下文对象
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            WorkContext.Configure(httpContextAccessor);
            return app;
        }

        /// <summary>
        /// 启用静态配置
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static void UseStaticConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        {
            ConfigHelper.SetStaticConfiguration(configuration);
        }
    }
}
