using System;
using Dragon.Framework.Core.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        public static void AddMemoryCache(this IServiceCollection services,
            IConfiguration configuration, bool useHybridMode = false)
        {
            MemoryCacheBootstrap.UseMemoryCache(services, configuration, useHybridMode);
        }
    }
}
