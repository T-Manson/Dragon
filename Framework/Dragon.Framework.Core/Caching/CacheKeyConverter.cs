using Dragon.Framework.Core.Exceptions;
using Dragon.Framework.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Dragon.Framework.Core.Caching
{
    /// <summary>
    /// 缓存key转换类
    /// </summary>
    public static class CacheKeyConverter
    {
        /// <summary>
        /// 缓存区域
        /// </summary>
        private static string _region;

        /// <summary>
        /// 获取区域
        /// </summary>
        /// <returns></returns>
        public static string GetRegion()
        {
            return _region;
        }

        /// <summary>
        /// 设置区域
        /// </summary>
        /// <param name="configuration">配置</param>
        public static void SetRegion(IConfiguration configuration)
        {
            if (!_region.IsNullOrWhiteSpace()) return;

            string actual = configuration.GetValue<string>("Cache:Region");
            if (actual.IsNullOrWhiteSpace())
                actual = configuration.GetValue<string>("AppName");

            if (actual.IsNullOrWhiteSpace())
                throw new ConfigException("Cache:Region Or AppName");
            _region = actual;
        }

        /// <summary>
        /// 获取转换后的key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetKeyWithRegion(string key)
        {
            Guard.ArgumentNullOrWhiteSpaceString(key, "cache key不能为空");
            if (_region.IsNullOrWhiteSpace())
                return key;
            return $"{_region}:{key}";
        }
    }
}
