using Dragon.Framework.Caching.Memory;
using Dragon.Framework.Caching.Redis;
using Dragon.Framework.Core.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dragon.Framework.Caching.Hybrid
{
    /// <summary>
    /// 混合缓存ServiceCollection扩展
    /// </summary>
    public static class HybridCacheServiceCollectionExtensions
    {
        /// <summary>
        /// 注入HybridCache默认实例
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static void AddHybridCache(this IServiceCollection services, IConfiguration configuration)
        {
            HybridCacheBootstrap.UseHybridCache(services, configuration);
        }
    }
}
