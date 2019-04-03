using System;
using Dragon.Framework.Core.Exceptions;
using Dragon.Framework.Core.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Dragon.Framework.MessageBus.Redis
{
    /// <summary>
    /// Redis消息帮助类
    /// </summary>
    public static class RedisMessageBusBootstrap
    {
        /// <summary>
        /// 启用Redis Bus
        /// </summary>
        /// <param name="configuration">配置</param>
        public static void UseRedisBus(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisMessageBusOptions>(options =>
            {
                RedisMessageBusBootstrap.SetRedisMessageBusOptions(configuration, options);
            });

            services.AddSingleton<IMessageBus, DefaultRedisMessageBus>();
            Console.WriteLine("Redis Bus 注入完成。");
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="options">配置对象</param>
        internal static void SetRedisMessageBusOptions(IConfiguration configuration, RedisMessageBusOptions options)
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
        }
    }
}
