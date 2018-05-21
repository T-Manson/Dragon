using System;
using System.Runtime.Serialization;

namespace Dragon.Framework.Core.Exceptions
{
    /// <summary>
    /// RPC异常
    /// </summary>
    [Serializable]
    public class RpcException : Exception
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public RpcException() : base("RPC Error.") { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        public RpcException(string errorMessage) : base(errorMessage) { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="msgFormat">带占位符的消息</param>
        /// <param name="os">占位数据</param>
        public RpcException(string msgFormat, params object[] os) : base(string.Format(msgFormat, os)) { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">错误来源</param>
        public RpcException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// 自定义错误消息的构造函数
        /// </summary>
        /// <param name="info">消息</param>
        /// <param name="context">错误来源</param>
        protected RpcException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
