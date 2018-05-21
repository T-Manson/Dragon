using System;
using System.Threading.Tasks;

namespace Dragon.Framework.Core.MessageBus
{
    /// <summary>
    /// 消息接口
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// 发布消息至指定频道
        /// </summary>
        /// <returns></returns>
        /// <param name="channel">频道</param>
        /// <param name="message">消息体</param>
        void Publish<T>(string channel, T message);

        /// <summary>
        /// 异步发布消息至指定频道
        /// </summary>
        /// <returns></returns>
        /// <param name="channel">频道</param>
        /// <param name="message">消息体</param>
        Task PublishAsync<T>(string channel, T message);

        /// <summary>
        /// 订阅指定频道
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="handler">处理消息的方法</param>
        void Subscribe<T>(string channel, Action<T> handler);

        /// <summary>
        /// 异步订阅指定频道
        /// </summary>
        /// <returns></returns>
        /// <param name="channel">频道</param>
        /// <param name="handler">处理消息的方法</param>
        Task SubscribeAsync<T>(string channel, Action<T> handler);
    }
}
