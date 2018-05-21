using System;
using System.Runtime.Serialization;

namespace Dragon.Framework.Core.Exceptions
{
    /// <summary>
    /// 没有权限
    /// </summary>
    [Serializable]
    public class NoPermissionException : Exception
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public NoPermissionException() : base("No Permission.") { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        public NoPermissionException(string errorMessage) : base(errorMessage) { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="msgFormat">带占位符的消息</param>
        /// <param name="os">占位数据</param>
        public NoPermissionException(string msgFormat, params object[] os) : base(string.Format(msgFormat, os)) { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">错误来源</param>
        public NoPermissionException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="info">消息</param>
        /// <param name="context">错误来源</param>
        protected NoPermissionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
