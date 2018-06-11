using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dragon.Framework.ApiCore.Models;
using Dragon.Framework.Core.Models;
using Dragon.Framework.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Dragon.Framework.ApiCore.Common
{
    /// <summary>
    /// 基础Api控制器
    /// </summary>
    public class ApiController : Controller
    {
        #region Success

        /// <summary>
        /// 成功（无data）
        /// </summary>
        /// <param name="code">结果码</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected object SuccessWithNoData(int code = 0, string message = null)
        {
            return GetResponse(code == 0 ? (int)HttpStatusCode.OK : code, default(object), message);
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="code">结果码</param>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected object Success<T>(T data, int code = 0, string message = null)
        {
            return GetResponse(code == 0 ? (int)HttpStatusCode.OK : code, data, message);
        }

        /// <summary>
        /// 成功（列表）
        /// </summary>
        /// <param name="code">结果码</param>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected object Success<T>(ResultModel<T> data, int code = 0, string message = null)
        {
            if (data == null)
                return GetResponse(code == 0 ? (int)HttpStatusCode.OK : code, new PagedListDataModel<T>(), message);

            if (!data.ConsoleMsg.IsNullOrWhiteSpace())
                Console.WriteLine(data.ConsoleMsg);

            return GetResponse(code == 0 ? (int)HttpStatusCode.OK : code, new PagedListDataModel<T>(data.Result, data.Total), message);
        }

        /// <summary>
        /// 成功（列表）
        /// </summary>
        /// <param name="code">结果码</param>
        /// <param name="data">数据集合</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected object Success<T>(List<T> data, int code = 0, string message = null)
        {
            if (data == null)
                return GetResponse(code == 0 ? (int)HttpStatusCode.OK : code, new PagedListDataModel<T>(), message);
            return GetResponse(code == 0 ? (int)HttpStatusCode.OK : code, new PagedListDataModel<IEnumerable<T>>(data, data.Count), message);
        }

        /// <summary>
        /// 成功（列表）
        /// </summary>
        /// <param name="code">结果码</param>
        /// <param name="data">数据集合</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected object Success<T>(IList<T> data, int code = 0, string message = null)
        {
            if (data == null)
                return GetResponse(code == 0 ? (int)HttpStatusCode.OK : code, new PagedListDataModel<T>(), message);
            return GetResponse(code == 0 ? (int)HttpStatusCode.OK : code, new PagedListDataModel<IEnumerable<T>>(data, data.Count), message);
        }

        /// <summary>
        /// 成功（列表）
        /// </summary>
        /// <param name="code">结果码</param>
        /// <param name="data">数据集合</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected object Success<T>(IEnumerable<T> data, int code = 0, string message = null)
        {
            if (data == null)
                return GetResponse(code == 0 ? (int)HttpStatusCode.OK : code, new PagedListDataModel<T>(), message);

            var list = data.ToList();
            return GetResponse(code == 0 ? (int)HttpStatusCode.OK : code, new PagedListDataModel<IEnumerable<T>>(list, list.Count), message);
        }

        #endregion

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected object Error(int code, string message = null)
        {
            return GetResponse(code, default(object), message);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected object Failure(int code, string message = null)
        {
            return GetResponse(code, default(object), message);
        }

        /// <summary>
        /// 获取注入的服务，不存在则为null
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns></returns>
        protected T GetService<T>()
        {
            return WorkContext.Current.RequestServices.GetService<T>();
        }

        /// <summary>
        /// 获取注入的服务，不存在则throw exception
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns></returns>
        protected T GetRequiredService<T>()
        {
            return WorkContext.Current.RequestServices.GetRequiredService<T>();
        }

        #region 公共方法

        /// <summary>
        /// 包装response结果
        /// </summary>
        /// <param name="code">结果码</param>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        private object GetResponse<T>(int code, T data, string message)
        {
            var response = new ApiResponse<T>(code, data, message);
            return Json(response);
        }

        #endregion
    }
}
