using System;

namespace Dragon.Framework.Core.MessageBus.CacheMessageModel
{
    /// <summary>
    /// 缓存同步消息
    /// </summary>
    public class CacheMessage
    {
        /// <summary>
        /// 服务器标识
        /// </summary>
        public string ServerId { get; set; }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 缓存值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan? Expiration { get; set; }

        /// <summary>
        /// 缓存消息类型
        /// </summary>
        public CacheMessageType CacheMessageType { get; set; }

        /// <summary>
        /// 设置添加消息
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="expiration"></param>
        public static CacheMessage SetAddMessage(string serverId, string cacheKey, object cacheValue, TimeSpan? expiration)
        {
            return new CacheMessage
            {
                ServerId = serverId,
                Key = cacheKey,
                Value = cacheValue,
                Expiration = expiration,
                CacheMessageType = CacheMessageType.Add
            };
        }

        /// <summary>
        /// 设置更新消息
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="expiration"></param>
        public static CacheMessage SetUpdateMessage(string serverId, string cacheKey, object cacheValue, TimeSpan? expiration)
        {
            return new CacheMessage
            {
                ServerId = serverId,
                Key = cacheKey,
                Value = cacheValue,
                Expiration = expiration,
                CacheMessageType = CacheMessageType.Update
            };
        }

        /// <summary>
        /// 设置删除消息
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="cacheKey"></param>
        public static CacheMessage SetDeleteMessage(string serverId, string cacheKey)
        {
            return new CacheMessage
            {
                ServerId = serverId,
                Key = cacheKey,
                CacheMessageType = CacheMessageType.Delete
            };
        }
    }
}
