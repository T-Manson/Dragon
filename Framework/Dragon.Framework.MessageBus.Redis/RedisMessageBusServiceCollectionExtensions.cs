using System;
using Dragon.Framework.Core.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            RedisMessageBusBootstrap.UseRedisBus(services, configuration);
        }
    }
}
