using Dragon.Framework.Core.Environment.State;
using Dragon.Framework.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dragon.Framework.Core.Environment
{
    /// <summary>
    /// 请求上下文
    /// </summary>
    public static class WorkContext
    {
        /// <summary>
        /// Net Core默认上下文
        /// </summary>
        private static IHttpContextAccessor _accessor;

        /// <summary>
        /// 状态集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, IWorkContextState> StateDictionary = new ConcurrentDictionary<string, IWorkContextState>();

        /// <summary>
        /// 当前请求上下文
        /// </summary>
        public static HttpContext Current => _accessor.HttpContext;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="accessor">Net Core默认上下文</param>
        public static void Configure(IHttpContextAccessor accessor)
        {
            Guard.ArgumentNotNull(accessor, nameof(accessor));

            _accessor = accessor;
            Console.WriteLine("上下文启用完成。");
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stateName">状态名称</param>
        /// <returns></returns>
        public static T GetState<T>(string stateName) where T : IWorkContextState
        {
            var state = StateDictionary.GetOrAdd(stateName, sName =>
            {
                var workContextStateList = Current.RequestServices.GetRequiredService<IEnumerable<IWorkContextState>>();
                return workContextStateList.First(s => s.StateName.CaseSensitiveEquals(stateName));
            });

            return (T)state;
        }
    }
}
