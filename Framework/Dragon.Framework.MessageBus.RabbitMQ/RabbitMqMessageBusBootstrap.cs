using Dragon.Framework.Core.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Dragon.Framework.MessageBus.RabbitMQ
{
    /// <summary>
    /// Redis消息帮助类
    /// </summary>
    public static class RabbitMqMessageBusBootstrap
    {
        ///// <summary>
        ///// 启用Redis Bus TODO
        ///// </summary>
        ///// <param name="configuration">配置</param>
        //public static void UseRedisBus(IConfiguration configuration)
        //{
        //    RedisMessageBusOptions options = new RedisMessageBusOptions();
        //    SetRedisMessageBusOptions(configuration, options);

        //    // 设置配置
        //    DependencyConfigurator.RegisterInstance<IOptions<RedisMessageBusOptions>, RedisMessageBusOptions>(options);

        //    DependencyConfigurator.RegisterType<IMessageBus, DefaultRedisMessageBus>();
        //    Console.WriteLine("Redis Bus注入完成。");
        //}

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="options">配置对象</param>
        internal static void SetRedisMessageBusOptions(IConfiguration configuration, RabbitMqMessageBusOptions options)
        {
            string uri = configuration.GetValue<string>("RabbitMQ.Default.Uri");
            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ConfigException("RabbitMQ.Default.Uri");
            }
            string username = configuration.GetValue<string>("RabbitMQ.Default.Username");
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ConfigException("RabbitMQ.Default.Username");
            }
            string password = configuration.GetValue<string>("RabbitMQ.Default.Password");
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ConfigException("RabbitMQ.Default.Password");
            }
            string exchange = configuration.GetValue<string>("RabbitMQ.Default.Exchange");
            if (!string.IsNullOrWhiteSpace(exchange))
            {
                throw new ConfigException("RabbitMQ.Default.Exchange");
            }

            options.Uri = uri;
            options.Username = username;
            options.Password = password;
            options.Exchange = exchange;
        }
    }
}
