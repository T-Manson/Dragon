using Dragon.Framework.Core.Caching;
using Dragon.Framework.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Threading;

namespace Dragon.Framework.Caching.Redis
{
    /// <summary>
    /// Redis管理类
    /// </summary>
    public class DefaultRedisCacheManager : IRedisCacheManager
    {
        /// <summary>
        /// 分布式缓存
        /// </summary>
        public CacheLevel CacheLevel => CacheLevel.Distributed;

        /// <summary>
        /// 配置
        /// </summary>
        private readonly RedisCacheOptions _options;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly Lazy<ILogger> _loggerLazy;

        /// <summary>
        /// 连接
        /// </summary>
        private Lazy<ConnectionMultiplexer> _connectionLazy;

        /// <summary>
        /// redis DB
        /// </summary>
        private IDatabase _database;

        /// <summary>
        /// 单例对象锁
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// 是否已经回收
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 空值常量key
        /// </summary>
        private const string NullDataValue = "null";

        private const string NullDataKeyFormat = "{0}:nodata";

        private const string MutexKeyFormat = "{0}:mutex";

        /// <summary>
        /// String containing the Lua unlock script.
        /// </summary>
        private const String UnlockScript = @"
            if redis.call(""get"",KEYS[1]) == ARGV[1] then
                return redis.call(""del"",KEYS[1])
            else
                return 0
            end";

        #region 初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options"></param>
        /// <param name="loggerFactory"></param>
        public DefaultRedisCacheManager(IOptions<RedisCacheOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options.Value;
            _loggerLazy = new Lazy<ILogger>(() => loggerFactory?.CreateLogger<DefaultRedisCacheManager>() ?? (ILogger)NullLogger<DefaultRedisCacheManager>.Instance);
            _connectionLazy = new Lazy<ConnectionMultiplexer>(CreateConnection, true);
        }

        #endregion

        /// <summary>
        /// 获取Database
        /// </summary>
        /// <returns></returns>
        protected IDatabase GetDatebase()
        {
            if (_database == null)
            {
                lock (Locker)
                {
                    if (_database == null)
                    {
                        _database = _connectionLazy.Value.GetDatabase(_options.Db);
                    }
                }
            }
            return _database;
        }

        /// <summary>
        /// 创建唯一锁Id
        /// </summary>
        /// <returns></returns>
        protected static byte[] CreateUniqueLockId()
        {
            return Guid.NewGuid().ToByteArray();
        }

        #region String

        public T GetOrAdd<T>(string key, Func<T> getData, TimeSpan? expiry = null)
        {
            var value = GetDatebase().StringGet(CacheKeyConverter.GetKeyWithRegion(key));
            if (value.HasValue) return GetValue<T>(value);

            var mutexKey = string.Format(MutexKeyFormat, key);

            // 防击穿（热点key并发）
            var uniqueValue = CreateUniqueLockId();
            if (SetNotExists(mutexKey, uniqueValue, TimeSpan.FromMinutes(1)))
            {
                var result = getData();
                _loggerLazy.Value.LogInformation($"GetOrAdd, [key:{key}], call getData func");

                //防穿透
                if (result == null)
                    Set(key, default(T), TimeSpan.FromSeconds(1));
                else
                    Set(key, value, expiry);

                UnlockInstance(mutexKey, uniqueValue);

                return result;
            }

            Thread.Sleep(50);
            return GetOrAdd(key, getData, expiry);
        }

        public T Get<T>(string key)
        {
            return GetValue<T>(GetDatebase().StringGet(CacheKeyConverter.GetKeyWithRegion(key)));
        }

        public bool Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            return GetDatebase().StringSet(CacheKeyConverter.GetKeyWithRegion(key), JsonHelper.SerializeObject(value), expiry);
        }

        #endregion

        #region Key

        public bool Delete(string key)
        {
            return GetDatebase().KeyDelete(CacheKeyConverter.GetKeyWithRegion(key));
        }

        #endregion

        #region 获取结果公共方法

        /// <summary>
        /// 获取缓存结果
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="redisValue">缓存数据</param>
        /// <returns></returns>
        private T GetValue<T>(RedisValue redisValue)
        {
            return redisValue.HasValue ? JsonHelper.DeserializeObject<T>(redisValue) : default(T);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 解锁实例
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        private void UnlockInstance(string key, byte[] value)
        {
            RedisKey[] keys = { CacheKeyConverter.GetKeyWithRegion(key) };
            RedisValue[] values = { value };
            GetDatebase().ScriptEvaluate(
                UnlockScript,
                keys,
                values
            );
        }

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

        /// <summary>
        /// 设置不存在的值
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        private bool SetNotExists(string key, byte[] value, TimeSpan? expiry = null)
        {
            return GetDatebase().StringSet(CacheKeyConverter.GetKeyWithRegion(key), value, expiry, When.NotExists);
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
                _database = null;
                _connectionLazy?.Value?.Dispose();
                _connectionLazy = null;
            }

            _disposed = true;
        }

        #endregion
    }
}
