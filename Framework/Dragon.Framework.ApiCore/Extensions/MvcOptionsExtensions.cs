using Dragon.Framework.ApiCore.Conventions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Dragon.Framework.ApiCore.Extensions
{
    /// <summary>
    /// Mvc配置扩展
    /// </summary>
    public static class MvcOptionsExtensions
    {
        /// <summary>
        /// 启用全局Route Prefix
        /// </summary>
        /// <param name="mvcOptions"></param>
        /// <param name="prefix">前缀</param>
        public static void UseGlobalRoutePrefix(this MvcOptions mvcOptions, string prefix)
        {
            //添加自定义全局Route约定
            mvcOptions.Conventions.Add(new GlobalRoutePrefixConvention(new RouteAttribute(prefix)));
        }

        /// <summary>
        /// 启用Api
        /// </summary>
        /// <param name="mvcOptions"></param>
        public static void UseApi(this MvcOptions mvcOptions)
        {
            //添加自定义Api Action参数模型绑定约定
            mvcOptions.Conventions.Add(new ApiActionParameterBindingConvention());
        }
    }
}
