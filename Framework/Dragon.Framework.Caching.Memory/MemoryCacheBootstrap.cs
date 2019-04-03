using System;
using Dragon.Framework.Core.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Dragon.Framework.Caching.Memory
{
    /// <summary>
    /// 本地缓存帮助类
    /// </summary>
    public static class MemoryCacheBootstrap
    {
        /// <summary>
        /// 启用本地缓存
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="useHybridMode">是否启用混合模式</param>
        public static void UseMemoryCache(IServiceCollection services,
            IConfiguration configuration, bool useHybridMode = false)
        {
            if (useHybridMode)
                services.AddSingleton<IMemoryCacheManager, DefaultMemoryCacheManager>();
            else
                services.AddSingleton<ICacheManager, DefaultMemoryCacheManager>();

            Console.WriteLine("MemoryCache 注入完成。");
        }
    }
}
