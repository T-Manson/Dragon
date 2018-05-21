using Dragon.Framework.Core.Caching;
using Dragon.Framework.Core.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Dragon.Framework.Caching.Redis
{
    /// <summary>
    /// Redis帮助类
    /// </summary>
    public static class RedisBootstrap
    {
        ///// <summary>
        ///// 启用Redis TODO
        ///// 启用Redis TODO
        ///// </summary>
        ///// <param name="configuration">配置</param>
        ///// <param name="useHybridMode">是否启用混合模式</param>
        //public static void UseRedis(IConfiguration configuration, bool useHybridMode = false)
        //{
        //    RedisCacheOptions options = new RedisCacheOptions();
        //    SetRedisCacheOptions(configuration, options);

        //    // 设置配置
        //    DependencyConfigurator.RegisterInstance<IOptions<RedisCacheOptions>, RedisCacheOptions>(options);

        //    if (useHybridMode)
        //        DependencyConfigurator.RegisterType<IRedisCacheManager, DefaultRedisCacheManager>();
        //    else
        //        DependencyConfigurator.RegisterType<ICacheManager, DefaultRedisCacheManager>();

        //    Console.WriteLine("Redis注入完成。");
        //}

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="options">配置对象</param>
        internal static void SetRedisCacheOptions(IConfiguration configuration, RedisCacheOptions options)
        {
            string host = configuration.GetValue<string>("Cache:Redis:Host");
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ConfigException("Cache.Redis.Host");
            }
            string port = configuration.GetValue<string>("Cache:Redis:Port");
            if (string.IsNullOrWhiteSpace(port))
            {
                port = "6379";
            }
            string password = configuration.GetValue<string>("Cache:Redis:Password");
            string dbStr = configuration.GetValue<string>("Cache:Redis:Db");
            int db = 0;
            if (!string.IsNullOrWhiteSpace(dbStr) && int.TryParse(dbStr, out int result))
            {
                db = result;
            }

            options.Host = host;
            options.Port = port;
            options.Password = password;
            options.Db = db;

            // 设置缓存区域
            CacheKeyConverter.SetRegion(configuration);
        }
    }
}
