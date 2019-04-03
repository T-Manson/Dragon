using System;
using Dragon.Framework.Core.Caching;
using Dragon.Framework.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Dragon.Framework.Caching.Redis
{
    /// <summary>
    /// Redis帮助类
    /// </summary>
    public static class RedisBootstrap
    {
        /// <summary>
        /// 启用Redis
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="useHybridMode">是否启用混合模式</param>
        public static void UseRedis(IServiceCollection services,
            IConfiguration configuration, bool useHybridMode = false)
        {
            services.Configure<RedisCacheOptions>(options =>
            {
                RedisBootstrap.SetRedisCacheOptions(configuration, options);
            });

            // Hack StackExchange.Redis 1.2.6下使用单例模式会有连接线程池上限的BUG导致请求timeout
            // TODO 暂时使用Transient模式解决以上问题
            if (useHybridMode)
                services.AddTransient<IRedisCacheManager, DefaultRedisCacheManager>();
            else
                services.AddTransient<ICacheManager, DefaultRedisCacheManager>();

            Console.WriteLine("Redis 注入完成。");
        }

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
