using System;

namespace Dragon.Framework.Core.Caching
{
    /// <summary>
    /// 缓存管理接口
    /// </summary>
    public interface ICacheManager
    {
        #region String

        #region GetOrAdd

        /// <summary>
        /// 获取key的数据，不存在则读取数据做一次缓存
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="getData">获取数据的方法</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        T GetOrAdd<T>(string key, Func<T> getData, TimeSpan? expiry = null);

        #endregion

        #region Get

        /// <summary>
        /// 获取获取key的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        T Get<T>(string key);

        #endregion

        #region Set

        /// <summary>
        /// 设置String数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        bool Set<T>(string key, T value, TimeSpan? expiry = null);

        #endregion

        #endregion

        #region Key

        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        bool Delete(string key);

        #endregion
    }
}
