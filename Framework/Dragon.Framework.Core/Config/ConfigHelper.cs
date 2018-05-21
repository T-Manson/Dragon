using Microsoft.Extensions.Configuration;

namespace Dragon.Framework.Core.Config
{
    /// <summary>
    /// 配置帮助
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Local Configuration
        /// </summary>
        private static IConfiguration _configuration;

        /// <summary>
        /// 设置静态配置
        /// </summary>
        public static void SetStaticConfiguration(IConfiguration configuration)
        {
            if (configuration != null) _configuration = configuration;
        }

        /// <summary>
        /// 从配置中心中获取指定key的值
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static T GetValue<T>(string key)
        {
            return _configuration.GetValue<T>(ChangeKey(key));
        }

        /// <summary>
        /// 从配置中心中获取指定key的值
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T GetValue<T>(string key, T defaultValue)
        {
            return _configuration.GetValue(ChangeKey(key), defaultValue);
        }

        /// <summary>
        /// 从配置中心中获取指定key的值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            return GetValue<string>(key);
        }

        /// <summary>
        /// 从配置中心中获取指定key的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetValue(string key, string defaultValue)
        {
            return GetValue<string>(key, defaultValue);
        }

        #region 公共方法

        private static string ChangeKey(string key)
        {
            return key.Trim().Replace('.', ':');
        }

        #endregion
    }
}
