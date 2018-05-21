using Dragon.Framework.Core.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dragon.Framework.Caching.Redis
{
    /// <summary>
    /// Redis ServiceCollection扩展
    /// </summary>
    public static class RedisServiceCollectionExtensions
    {
        /// <summary>
        /// 注入Redis默认实例
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="configuration">配置</param>
        /// <param name="useHybridMode">是否启用混合模式</param>
        /// <returns></returns>
        public static void AddRedis(this IServiceCollection services,
            IConfiguration configuration, bool useHybridMode = false)
        {
            services.Configure<RedisCacheOptions>(options =>
            {
                RedisBootstrap.SetRedisCacheOptions(configuration, options);
            });

            if (useHybridMode)
                services.AddSingleton<IRedisCacheManager, DefaultRedisCacheManager>();
            else
                services.AddSingleton<ICacheManager, DefaultRedisCacheManager>();

            Console.WriteLine("Redis注入完成。");
        }
    }
}
