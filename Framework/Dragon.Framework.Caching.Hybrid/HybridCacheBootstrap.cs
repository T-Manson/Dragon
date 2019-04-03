using Dragon.Framework.Core.Caching;
using Dragon.Framework.Caching.Redis;
using Dragon.Framework.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dragon.Framework.Caching.Hybrid
{
    /// <summary>
    /// 混合缓存帮助类
    /// </summary>
    public static class HybridCacheBootstrap
    {
        /// <summary>
        /// 启用混合缓存
        /// </summary>
        /// <param name="configuration">配置</param>
        public static void UseHybridCache(IServiceCollection services, IConfiguration configuration)
        {
            MemoryCacheBootstrap.UseMemoryCache(services, configuration, true);
            RedisBootstrap.UseRedis(services, configuration, true);
            services.AddSingleton<ICacheManager, DefaultHybridCacheManager>();
            Console.WriteLine("HybridCache 注入完成。");
        }
    }
}
