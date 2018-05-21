using Dragon.Framework.ApiCore.Enums;
using Dragon.Framework.ApiCore.Models;
using Dragon.Framework.Core.Exceptions;
using Dragon.Framework.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dragon.Framework.ApiCore.Middlewares
{
    /// <summary>
    /// 异常中间件
    /// </summary>
    public class ExceptionMiddleware
    {
        /// <summary>
        /// 请求委托
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next"></param>
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// 触发
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 捕获异常
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常</param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            await WriteExceptionAsync(context, exception).ConfigureAwait(false);
        }

        /// <summary>
        /// 将异常结果写入返回
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常</param>
        /// <returns></returns>
        private async Task WriteExceptionAsync(HttpContext context, Exception exception)
        {
            //返回友好的提示
            var response = context.Response;

            //返回结果
            ApiResponse responseContent;
            if (exception is DalException dalException)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseContent = ApiResponse.DbFailure(dalException);
            }
            else if (exception is RpcException rpcException)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseContent = ApiResponse.RpcFailure(rpcException);
            }
            else if (exception is NotLoginException)
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                responseContent = ApiResponse.NotLogin("未登录。");
            }
            else if (exception is NoPermissionException)
            {
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                responseContent = ApiResponse.NoPermission("没有操作权限。");
            }
            else if (exception is ArgumentException)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                responseContent = new ApiResponse((int)ResponseCode.ArgumentFailure, null, exception.Message);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseContent = new ApiResponse((int)HttpStatusCode.InternalServerError, null, exception.ToString());
            }

            response.ContentType = context.Request.Headers["Accept"];
            if (response.ContentType.ToLower() == "application/xml")
            {
                await response.WriteAsync(Object2XmlString(responseContent)).ConfigureAwait(false);
            }
            else
            {
                response.ContentType = "application/json";
                await response.WriteAsync(responseContent.ToJson()).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 对象转为Xml
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private string Object2XmlString(object o)
        {
            var sw = new StringWriter();
            try
            {
                var serializer = new XmlSerializer(o.GetType());
                serializer.Serialize(sw, o);
            }
            catch
            {
                //Handle Exception Code
            }
            finally
            {
                sw.Dispose();
            }
            return sw.ToString();
        }
    }
}
