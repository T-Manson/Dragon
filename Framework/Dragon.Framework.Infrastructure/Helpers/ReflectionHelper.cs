using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dragon.Framework.Infrastructure.Helpers
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// 实体类属性缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, IEnumerable<string>> ClassPropertyNameCache = new ConcurrentDictionary<Type, IEnumerable<string>>();

        /// <summary>
        /// 获取实体类所有属性名称
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns></returns>
        public static IEnumerable<string> GetPropertyNameList(Type type)
        {
            if (!type.IsClass) return new List<string>();
            if (ClassPropertyNameCache.TryGetValue(type, out IEnumerable<string> resultList)) return resultList;

            resultList = type.GetProperties().Select(property => property.Name).ToList();
            ClassPropertyNameCache.TryAdd(type, resultList);

            return resultList;

        }
    }
}
