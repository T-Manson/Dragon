using Dragon.Framework.Core.MessageBus;
using Dragon.Framework.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Dragon.Framework.MessageBus.Redis
{
    /// <summary>
    /// Redis消息管理类
    /// </summary>
    public class DefaultRedisMessageBus : IMessageBus, IDisposable
    {
        /// <summary>
        /// 配置
        /// </summary>
        private readonly RedisMessageBusOptions _options;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly Lazy<ILogger> _loggerLazy;

        /// <summary>
        /// 连接
        /// </summary>
        private Lazy<ConnectionMultiplexer> _connectionLazy;

        /// <summary>
        /// redis subscriber
        /// </summary>
        private ISubscriber _subscriber;

        /// <summary>
        /// 单例对象锁
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// 是否已经回收
        /// </summary>
        private bool _disposed;

        #region 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options"></param>
        /// <param name="loggerFactory"></param>
        public DefaultRedisMessageBus(IOptions<RedisMessageBusOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options.Value;
            _loggerLazy = new Lazy<ILogger>(() => loggerFactory?.CreateLogger<DefaultRedisMessageBus>() ?? (ILogger)NullLogger<DefaultRedisMessageBus>.Instance);
            _connectionLazy = new Lazy<ConnectionMultiplexer>(CreateConnection, true);
        }

        #endregion

        /// <summary>
        /// 获取Subscriber
        /// </summary>
        /// <returns></returns>
        protected ISubscriber GetSubscriber()
        {
            if (_subscriber == null)
            {
                lock (Locker)
                {
                    if (_subscriber == null)
                    {
                        _connectionLazy.Value.PreserveAsyncOrder = false;// 并行
                        _subscriber = _connectionLazy.Value.GetSubscriber();
                    }
                }
            }
            return _subscriber;
        }

        #region 发布/订阅

        /// <summary>
        /// 发布消息至指定频道
        /// </summary>
        /// <returns></returns>
        /// <param name="channel">频道</param>
        /// <param name="message">消息体</param>
        public void Publish<T>(string channel, T message)
        {
            GetSubscriber().Publish(channel, JsonHelper.SerializeObject(message));
        }

        /// <summary>
        /// 异步发布消息至指定频道
        /// </summary>
        /// <returns></returns>
        /// <param name="channel">频道</param>
        /// <param name="message">消息体</param>
        public Task PublishAsync<T>(string channel, T message)
        {
            return GetSubscriber().PublishAsync(channel, JsonHelper.SerializeObject(message));
        }

        /// <summary>
        /// 订阅指定频道
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="handler">处理消息的方法</param>
        public void Subscribe<T>(string channel, Action<T> handler)
        {
            GetSubscriber().Subscribe(channel, (redisChannel, value) =>
                handler(JsonHelper.DeserializeObject<T>(value.ToString())));
        }

        /// <summary>
        /// 异步订阅指定频道
        /// </summary>
        /// <returns></returns>
        /// <param name="channel">频道</param>
        /// <param name="handler">处理消息的方法</param>
        public Task SubscribeAsync<T>(string channel, Action<T> handler)
        {
            return GetSubscriber().SubscribeAsync(channel, (redisChannel, value) =>
                handler(JsonHelper.DeserializeObject<T>(value.ToString())));
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer CreateConnection()
        {
            try
            {
                ConfigurationOptions configurationOptions = ConfigurationOptions.Parse($"{_options.Host}:{_options.Port}");
                if (!string.IsNullOrWhiteSpace(_options.Password))
                    configurationOptions.Password = _options.Password;
                return ConnectionMultiplexer.Connect(configurationOptions);
            }
            catch (Exception ex)
            {
                _loggerLazy.Value.LogError($"Redis连接失败。Exception:{ex}");
                throw;
            }
        }

        #endregion

        #region 回收

        /// <summary>
        /// 回收
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="disposing">是否回收内部资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _subscriber?.UnsubscribeAll();
                _subscriber = null;
                _connectionLazy?.Value?.Close();
                _connectionLazy?.Value?.Dispose();
                _connectionLazy = null;
            }

            _disposed = true;
        }

        #endregion
    }
}
