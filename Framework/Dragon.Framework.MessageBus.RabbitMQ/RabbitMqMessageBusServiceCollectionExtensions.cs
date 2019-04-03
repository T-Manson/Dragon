using System;
using Dragon.Framework.Core.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dragon.Framework.MessageBus.RabbitMQ
{
    /// <summary>
    /// RabbitMq ServiceCollection扩展
    /// </summary>
    public static class RabbitMqMessageBusServiceCollectionExtensions
    {
        /// <summary>
        /// 注入RabbitMq Bus默认实例
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            RabbitMqMessageBusBootstrap.UseRabbitMq(services, configuration);
        }
    }
}
