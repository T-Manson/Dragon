using Dragon.Framework.Core.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dragon.Framework.Caching.Memory
{
    /// <summary>
    /// 内存缓存ServiceCollection扩展
    /// </summary>
    public static class MemoryCacheServiceCollectionExtensions
    {
        /// <summary>
        /// 注入MemoryCache默认实例
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="configuration">配置</param>
        /// <param name="useHybridMode">是否启用混合模式</param>
        /// <returns></returns>
        public static void AddLocalMemoryCache(this IServiceCollection services,
            IConfiguration configuration, bool useHybridMode = false)
        {
            services.AddMemoryCache(options => configuration.GetSection("Cache.MemoryCache"));

            if (useHybridMode)
                services.AddSingleton<IMemoryCacheManager, DefaultMemoryCacheManager>();
            else
                services.AddSingleton<ICacheManager, DefaultMemoryCacheManager>();


            Console.WriteLine("MemoryCache注入完成。");
        }
    }
}
