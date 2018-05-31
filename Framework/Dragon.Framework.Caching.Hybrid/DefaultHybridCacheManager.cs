using Dragon.Framework.Caching.Memory;
using Dragon.Framework.Caching.Redis;
using Dragon.Framework.Core.Caching;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dragon.Framework.Caching.Hybrid
{
    /// <summary>
    /// 混合缓存帮助类
    /// </summary>
    public class DefaultHybridCacheManager : IHybridCacheManager
    {
        /// <summary>
        /// 混合缓存提供者
        /// </summary>
        private readonly List<IHybridCacheProvider> _providers;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly Lazy<ILogger> _loggerLazy;

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
        /// <param name="loggerFactory"></param>
        public DefaultHybridCacheManager(IMemoryCacheManager memoryCacheManager, IRedisCacheManager redisCacheManager, ILoggerFactory loggerFactory)
        {
            _providers = new List<IHybridCacheProvider> { memoryCacheManager, redisCacheManager };
            _loggerLazy = new Lazy<ILogger>(() => loggerFactory?.CreateLogger<DefaultHybridCacheManager>() ?? (ILogger)NullLogger<DefaultHybridCacheManager>.Instance);
        }

        #endregion

        #region String

        public T GetOrAdd<T>(string key, Func<T> getData, TimeSpan? expiry = null)
        {
            var valueTuple = GetTupleByGetOrAdd(key, getData, expiry);
            var hasValueIndex = valueTuple.Item2;

            for (; hasValueIndex >= 0; hasValueIndex--)
            {
                _providers[hasValueIndex].Set(key, valueTuple.Item1);
            }

            return valueTuple.Item1;
        }

        public T Get<T>(string key)
        {
            var value = default(T);
            var hasValueIndex = 0;
            for (; hasValueIndex < _providers.Count; hasValueIndex++)
            {
                value = _providers[hasValueIndex].Get<T>(key);
                if (value != null && !value.Equals(default(T))) break;
            }

            for (; hasValueIndex >= 0; hasValueIndex--)
            {
                _providers[hasValueIndex].Set(key, value);
            }

            return value;
        }

        public bool Set<T>(string key, T value, TimeSpan? expiry = null)
        {
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
            var value = default(T);
            var hasValueIndex = 0;
            for (; hasValueIndex < _providers.Count; hasValueIndex++)
            {
                if (_providers[hasValueIndex].CacheLevel != CacheLevel.Distributed)
                {
                    value = _providers[hasValueIndex].Get<T>(key);
                    if (value != null && !value.Equals(default(T)))
                    {
                        hasValueIndex++;
                        break;
                    }
                }
                else
                {
                    value = _providers[hasValueIndex].GetOrAdd(key, getData, expiry);
                }
            }

            return new Tuple<T, int>(value, hasValueIndex - 2);
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
