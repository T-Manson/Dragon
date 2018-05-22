using Dragon.Framework.Core.Caching;
using Dragon.Framework.Core.Caching.MessageModel;
using Dragon.Framework.Core.MessageBus;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;

namespace Dragon.Framework.Caching.Memory
{
    /// <summary>
    /// 本地缓存帮助类
    /// </summary>
    public class DefaultMemoryCacheManager : IMemoryCacheManager
    {
        private const string CacheSyncChannel = "ygsc.ics.cache.sync";

        /// <summary>
        /// 本地缓存
        /// </summary>
        public CacheLevel CacheLevel => CacheLevel.Local;

        /// <summary>
        /// 内存缓存
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly Lazy<ILogger> _loggerLazy;

        /// <summary>
        /// 消息总线
        /// </summary>
        private readonly IMessageBus _messageBus;

        /// <summary>
        /// key集合
        /// </summary>
        private readonly ConcurrentDictionary<string, DateTime> _keyNullExpiry;

        /// <summary>
        /// 服务器标识
        /// </summary>
        private readonly string _serverId;

        #region 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="loggerFactory"></param>
        public DefaultMemoryCacheManager(IMemoryCache memoryCache, ILoggerFactory loggerFactory)
        {
            _memoryCache = memoryCache;
            _loggerLazy = new Lazy<ILogger>(() => loggerFactory?.CreateLogger<DefaultMemoryCacheManager>() ?? (ILogger)NullLogger<DefaultMemoryCacheManager>.Instance);
            _keyNullExpiry = new ConcurrentDictionary<string, DateTime>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="messageBus"></param>
        /// <param name="loggerFactory"></param>
        public DefaultMemoryCacheManager(IMemoryCache memoryCache, IMessageBus messageBus, ILoggerFactory loggerFactory)
        {
            _memoryCache = memoryCache;
            _messageBus = messageBus;
            _loggerLazy = new Lazy<ILogger>(() => loggerFactory?.CreateLogger<DefaultMemoryCacheManager>() ?? (ILogger)NullLogger<DefaultMemoryCacheManager>.Instance);
            _keyNullExpiry = new ConcurrentDictionary<string, DateTime>();
            _serverId = Guid.NewGuid().ToString("N");

            _messageBus.Subscribe<CacheMessage>(CacheSyncChannel, m =>
            {
                if (m.ServerId == _serverId) return;

                if (m.CacheMessageType != CacheMessageType.Delete) return;
                _memoryCache.Remove(m.Key);
                _loggerLazy.Value.LogInformation($"缓存同步，位置：{_serverId}，来源：{m.ServerId}，标识：[{m.Key}]，方式：delete");
            });
        }

        #endregion

        #region String

        /// <summary>
        /// 获取key的数据，不存在则读取数据做一次缓存
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="getData">获取数据的方法</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public T GetOrAdd<T>(string key, Func<T> getData, TimeSpan? expiry = null)
        {
            var nullKey = $"{key}:NullData";

            // 防止缓存雪崩
            if (_keyNullExpiry.TryGetValue(nullKey, out var expriy) && expriy >= DateTime.Now)
            {
                return default(T);
            }

            var value = Get<T>(key);
            if (value == null || value.Equals(default(T)))
            {
                value = getData();

                // 防止缓存雪崩
                if (value == null)
                {
                    var now = DateTime.Now.AddSeconds(5);
                    if (!_keyNullExpiry.TryAdd(nullKey, now))
                    {
                        _keyNullExpiry[nullKey] = now;
                    }
                }
                else
                {
                    Set(key, value, expiry);
                }
            }
            return value;
        }

        /// <summary>
        /// 获取获取key的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        /// <summary>
        /// 设置String数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            return SetAndPublish(key, value, expiry);
        }

        #endregion

        #region Key

        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            Remove(key);
            return true;
        }

        #endregion

        #region 基本公共方法

        /// <summary>
        /// 设置单个数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        private bool SetAndPublish<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (expiry.HasValue)
                _memoryCache.Set(key, value, expiry.Value);
            else
                _memoryCache.Set(key, value);
            _messageBus?.Publish(CacheSyncChannel, CacheMessage.SetDeleteMessage(_serverId, key));
            return true;
        }

        /// <summary>
        /// 删除单个数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        private void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        #endregion
    }
}
