using System;
using Dragon.Framework.ApiCore.Enums;
using Dragon.Framework.Core.Exceptions;

namespace Dragon.Framework.ApiCore.Models
{
    /// <summary>
    /// Api返回结果类
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// 状态描述
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 结果码
        /// </summary>
        public int Code { set; get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { set; get; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ApiResponse()
        {
            Data = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">结果码</param>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        public ApiResponse(int code, object data, string message = null)
        {
            Code = code;
            Status = GetStatus(code);
            Data = data;
            Message = message;
        }

        /// <summary>
        /// DB错误
        /// </summary>
        /// <param name="dalException">消息</param>
        /// <returns></returns>
        public static ApiResponse DbFailure(DalException dalException = null)
        {
            return new ApiResponse
            {
                Code = (int)ResponseCode.DBFailure,
                Status = "fail",
                Data = null,
                Message = dalException?.ToString()
            };
        }

        /// <summary>
        /// RPC错误
        /// </summary>
        /// <param name="rpcException">消息</param>
        /// <returns></returns>
        public static ApiResponse RpcFailure(RpcException rpcException = null)
        {
            return new ApiResponse
            {
                Code = (int)ResponseCode.RPCFailure,
                Status = "fail",
                Data = null,
                Message = rpcException?.ToString()
            };
        }

        /// <summary>
        /// 未登录
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static ApiResponse NotLogin(string message = null)
        {
            return new ApiResponse
            {
                Code = (int)ResponseCode.NotLogin,
                Status = "error",
                Data = null,
                Message = message
            };
        }

        /// <summary>
        /// 没有权限
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static ApiResponse NoPermission(string message = null)
        {
            return new ApiResponse
            {
                Code = (int)ResponseCode.NoPermission,
                Status = "error",
                Data = null,
                Message = message
            };
        }

        /// <summary>
        /// 获取状态描述
        /// </summary>
        /// <returns></returns>
        private string GetStatus(int code)
        {
            if (code < 400)
                //1XX、2XX、3XX
                return "success";
            if (code >= 400 && code < 500)
                return "error";
            return "fail";
        }
    }

    /// <summary>
    /// Api返回结果类
    /// </summary>
    public class ApiResponse<T> : ApiResponse
    {
        /// <summary>
        /// 数据
        /// </summary>
        public new T Data { set; get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ApiResponse()
        {
            Data = (T)Activator.CreateInstance(typeof(T));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">结果码</param>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        public ApiResponse(int code, T data, string message = null) : base(code, data, message)
        {
            Data = data;
        }
    }
}