using System;
using System.Runtime.Serialization;

namespace Dragon.Framework.Core.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// 配置异常
    /// </summary>
    [Serializable]
    public class ConfigException : Exception
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ConfigException() : base("缺少相关配置。") { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="configKey">配置key</param>
        public ConfigException(string configKey) : base($"缺少相关配置。{configKey}") { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="msgFormat">带占位符的消息</param>
        /// <param name="os">占位数据</param>
        public ConfigException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">错误来源</param>
        public ConfigException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="info">消息</param>
        /// <param name="context">错误来源</param>
        protected ConfigException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
