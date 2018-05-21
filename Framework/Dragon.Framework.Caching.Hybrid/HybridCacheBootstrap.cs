namespace Dragon.Framework.Caching.Hybrid
{
    /// <summary>
    /// 混合缓存帮助类
    /// </summary>
    public static class HybridCacheBootstrap
    {
        ///// <summary>
        ///// 启用混合缓存 TODO
        ///// </summary>
        ///// <param name="configuration">配置</param>
        //public static void UseHybridCache(IConfiguration configuration)
        //{
        //    MemoryCacheBootstrap.UseMemoryCache(configuration, true);
        //    RedisBootstrap.UseRedis(configuration, true);
        //    DependencyConfigurator.RegisterType<ICacheManager, DefaultHybridCacheManager>();
        //    Console.WriteLine("HybridCache注入完成。");
        //}
    }
}
