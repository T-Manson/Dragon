using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Dragon.Framework.Data.Dapper.Common
{
    /// <summary>
    /// 属性设置
    /// </summary>
    internal static class PropertySetter
    {
        /// <summary>
        /// 属性方法映射
        /// </summary>
        private static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> Cache
            = new ConcurrentDictionary<PropertyInfo, Action<object, object>>();

        /// <summary>
        /// 类型转换方法
        /// </summary>
        private static readonly MethodInfo ChangeType = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <typeparam name="E">目标类型</typeparam>
        /// <param name="property">属性</param>
        /// <param name="target">目标</param>
        /// <param name="value">值</param>
        public static void SetProperty<E>(PropertyInfo property, E target, object value) where E : class
        {
            var setter = Cache.GetOrAdd(property, prop =>
            {
                var p1 = Expression.Parameter(typeof(object));
                var p2 = Expression.Parameter(typeof(object));
                var call = Expression.Call(Expression.Convert(p1, typeof(E)), property.SetMethod,
                    Expression.Convert(Expression.Call(null, ChangeType, p2, Expression.Constant(prop.PropertyType)),
                        prop.PropertyType));
                var lambda = Expression.Lambda<Action<object, object>>(call, p1, p2);
                return lambda.Compile();
            });
            setter(target, value);
        }
    }
}
