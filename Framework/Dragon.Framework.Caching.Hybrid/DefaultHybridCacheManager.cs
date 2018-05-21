using Dragon.Framework.Caching.Memory;
using Dragon.Framework.Caching.Redis;
using Dragon.Framework.Core.Caching;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dragon.Framework.Caching.Hybrid
{
    /// <summary>
    /// 混合缓存帮助类
    /// </summary>
    public class DefaultHybridCacheManager : IHybridCacheManager
    {
        /// <summary>
        /// key集合
        /// </summary>
        private readonly ConcurrentDictionary<string, DateTime> _keyNullExpiry;

        /// <summary>
        /// 混合缓存提供者
        /// </summary>
        private readonly List<IHybridCacheProvider> _providers;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// 是否已经回收
        /// </summary>
        private bool _disposed;

        #region 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="memoryCacheManager">内存缓存</param>
        /// <param name="redisCacheManager">redis</param>
        /// <param name="logger"></param>
        public DefaultHybridCacheManager(IMemoryCacheManager memoryCacheManager, IRedisCacheManager redisCacheManager, ILogger logger)
        {
            _providers = new List<IHybridCacheProvider> { memoryCacheManager, redisCacheManager };

            _keyNullExpiry = new ConcurrentDictionary<string, DateTime>();

            _logger = logger;
        }

        #endregion

        #region String

        public T GetOrAdd<T>(string key, Func<T> getData, TimeSpan? expiry = null)
        {
            _logger.LogInformation($"Hybrid Cache Provider Count:{_providers.Count}");

            var nullKey = $"{key}:NullData";

            // 防止缓存雪崩
            if (_keyNullExpiry.TryGetValue(nullKey, out var expriy) && expriy >= DateTime.Now)
            {
                return default(T);
            }

            Tuple<T, int> valueTuple = GetTupleByGetOrAdd(key, getData, expiry);

            int hasValueIndex = valueTuple.Item2;
            for (; hasValueIndex > 0; hasValueIndex--)
            {
                _providers[hasValueIndex - 1].Set(key, valueTuple.Item1);
            }

            return valueTuple.Item1;
        }

        public T Get<T>(string key)
        {
            _logger.LogInformation($"Hybrid Cache Provider Count:{_providers.Count}");

            T value = default(T);
            int hasValueIndex = 0;
            for (; hasValueIndex < _providers.Count; hasValueIndex++)
            {
                value = _providers[hasValueIndex].Get<T>(key);
                if (value != null && !value.Equals(default(T))) break;
            }

            for (; hasValueIndex > 0; hasValueIndex--)
            {
                _providers[hasValueIndex - 1].Set(key, value);
            }

            return value;
        }

        public bool Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            _logger.LogInformation($"Hybrid Cache Provider Count:{_providers.Count}");

            foreach (var provider in _providers)
            {
                provider.Set(key, value, expiry);
            }
            return true;
        }

        #endregion

        #region Key

        public bool Delete(string key)
        {
            _logger.LogInformation($"Hybrid Cache Provider Count:{_providers.Count}");

            foreach (var provider in _providers)
            {
                provider.Delete(key);
            }
            return true;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取Hash类型数据中符合数据key的数据，不存在则读取数据做一次缓存
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="getData">获取数据的方法</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        private Tuple<T, int> GetTupleByGetOrAdd<T>(string key, Func<T> getData, TimeSpan? expiry = null)
        {
            T value = default(T);
            int hasValueIndex = 0;
            for (; hasValueIndex < _providers.Count; hasValueIndex++)
            {
                if (_providers[hasValueIndex].CacheLevel != CacheLevel.Distributed)
                {
                    value = _providers[hasValueIndex].Get<T>(key);
                    if (value != null && !value.Equals(default(T))) break;
                }
                else
                {
                    value = _providers[hasValueIndex].GetOrAdd(key, getData, expiry);

                    // 防止缓存雪崩
                    if (value == null || value.Equals(default(T)))
                    {
                        SetNullExpiry(key);
                        break;
                    }
                    else break;
                }
            }

            return new Tuple<T, int>(value, hasValueIndex);
        }

        /// <summary>
        /// 设置空数据有效期
        /// </summary>
        /// <param name="key"></param>
        private void SetNullExpiry(string key)
        {
            var nullKey = $"{key}:NullData";
            var now = DateTime.Now.AddSeconds(5);
            if (!_keyNullExpiry.TryAdd(nullKey, now))
            {
                _keyNullExpiry[nullKey] = now;
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
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var provider in _providers)
                {
                    if (provider is IDisposable providerWithDisposable)
                    {
                        providerWithDisposable.Dispose();
                    }
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
