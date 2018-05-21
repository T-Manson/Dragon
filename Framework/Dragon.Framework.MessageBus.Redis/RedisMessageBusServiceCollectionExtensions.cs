using Dragon.Framework.Core.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dragon.Framework.MessageBus.Redis
{
    /// <summary>
    /// Redis ServiceCollection扩展
    /// </summary>
    public static class RedisMessageBusServiceCollectionExtensions
    {
        /// <summary>
        /// 注入Redis Bus默认实例
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static void AddRedisBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisMessageBusOptions>(options =>
            {
                RedisMessageBusBootstrap.SetRedisMessageBusOptions(configuration, options);
            });

            services.AddSingleton<IMessageBus, DefaultRedisMessageBus>();
            Console.WriteLine("Redis Bus注入完成。");
        }
    }
}
