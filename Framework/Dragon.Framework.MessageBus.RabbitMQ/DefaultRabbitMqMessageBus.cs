using Dragon.Framework.Core.MessageBus;
using Dragon.Framework.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Framework.MessageBus.RabbitMQ
{
    /// <summary>
    /// Redis消息管理类
    /// </summary>
    public class DefaultRabbitMqMessageBus : IMessageBus, IDisposable
    {
        /// <summary>
        /// 配置
        /// </summary>
        private readonly RabbitMqMessageBusOptions _options;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// 连接
        /// </summary>
        private Lazy<IConnection> _connection;

        /// <summary>
        /// 是否已经回收
        /// </summary>
        private bool _disposed;

        #region 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public DefaultRabbitMqMessageBus(IOptions<RabbitMqMessageBusOptions> options, ILogger logger)
        {
            _options = options.Value;
            _logger = logger;
            _connection = new Lazy<IConnection>(CreateConnection, true);
        }

        #endregion

        #region 发布/订阅

        /// <summary>
        /// 发布消息至指定频道
        /// </summary>
        /// <returns></returns>
        /// <param name="channel">频道</param>
        /// <param name="message">消息体</param>
        public void Publish<T>(string channel, T message)
        {
            try
            {
                using (var channelModel = _connection.Value.CreateModel())
                {
                    var bytes = Encoding.UTF8.GetBytes(message.ToJson());
                    var basicProperties = channelModel.CreateBasicProperties();
                    basicProperties.DeliveryMode = 2;
                    channelModel.BasicPublish(_options.Exchange, channel, basicProperties, bytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Rmq: 消息发送失败， {message}。 Exception: {ex}");
            }
        }

        /// <summary>
        /// 异步发布消息至指定频道
        /// </summary>
        /// <returns></returns>
        /// <param name="channel">频道</param>
        /// <param name="message">消息体</param>
        public Task PublishAsync<T>(string channel, T message)
        {
            return Task.Run(() => { Publish(channel, message); });
        }

        /// <summary>
        /// 订阅指定频道
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="handler">处理消息的方法</param>
        public void Subscribe<T>(string channel, Action<T> handler)
        {
            try
            {
                using (var channelModel = _connection.Value.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channelModel);
                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            T body = JsonHelper.DeserializeObject<T>(Encoding.UTF8.GetString(ea.Body));
                            handler(body);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Rmq: 消息接受失败。message: {ea.Body}，Exception: {ex}");
                        }
                    };
                    channelModel.BasicConsume(channel, true, consumer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Rmq: 消息接受失败，Exception: {ex}");
            }
        }

        /// <summary>
        /// 异步订阅指定频道
        /// </summary>
        /// <returns></returns>
        /// <param name="channel">频道</param>
        /// <param name="handler">处理消息的方法</param>
        public Task SubscribeAsync<T>(string channel, Action<T> handler)
        {
            return Task.Run(() => { Subscribe(channel, handler); });
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        private IConnection CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory { Uri = new Uri(_options.Uri), UserName = _options.Username, Password = _options.Password };
                return factory.CreateConnection();
            }
            catch (Exception ex)
            {
                _logger.LogError($"RabbitMQ连接失败。 Exception: {ex}");
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
                _connection?.Value?.Dispose();
                _connection = null;
            }

            _disposed = true;
        }

        #endregion
    }
}
