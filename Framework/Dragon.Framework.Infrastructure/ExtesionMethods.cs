using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial class ExtensionMethods
    {
        public static object DefaultValue(this Type type)
        {
            var expression = Expression.Default(type);
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        public static T If<T>(this T item, Func<T, bool> predicate, T returnValue)
        {
            return predicate(item) ? returnValue : item;
        }

        #region ValueType

        public static T IfDefault<T>(this T item, T @default)
            where T : struct, IComparable<T>
        {
            return item.CompareTo(default(T)) == 0 ? @default : item;
        }

        public static T? IfDefaultReturnNull<T>(this T item)
            where T : struct, IComparable<T>
        {
            return item.CompareTo(default(T)) == 0 ? null : new T?(item);
        }

        public static T IfNullReturnDefault<T>(this T? item)
            where T : struct
        {
            return item ?? default(T);
        }

        public static T IfNull<T>(this T? value, T defaultValue)
            where T : struct
        {
            return value ?? defaultValue;
        }

        #endregion

        #region Object

        public static dynamic Reflect(this object instance)
        {
            return instance == null ? null : new ReflectionObject(instance);
        }

        public static bool SafeEquals(this object object1, object object2)
        {
            if (object1 == null && object2 == null)
            {
                return true;
            }
            else if (object1 == null)
            {
                return false;
            }
            else if (object2 == null)
            {
                return false;
            }
            else
            {
                return object1.Equals(object2);
            }
        }


        public static T IfNull<T>(this T item, T @default)
            where T : class
        {
            return item ?? @default;
        }

        private static bool PropertiesEquals(Type compareType, object x, object y,
            BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Instance,
            bool recurseComplexTypeProperties = true, Func<PropertyInfo, bool> predicate = null)
        {
            if (x == null && y == null || ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null) return false;
            var infos = compareType.GetTypeInfo().GetProperties(propertyFlags);
            foreach (var property in infos)
            {
                var index = property.GetIndexParameters();
                if (index.Length > 0)
                {
                    continue;
                }

                return CheckProperty(x, y, propertyFlags, recurseComplexTypeProperties, predicate, property);
            }
            return true;
        }

        private static bool CheckProperty(object x, object y, BindingFlags propertyFlags,
            bool recurseComplexTypeProperties, Func<PropertyInfo, bool> predicate,
            PropertyInfo property)
        {
            if (predicate != null && !predicate.Invoke(property))
            {
                return true;
            }

            var value1 = property.GetValue(x, null);
            var value = property.GetValue(y, null);

            if (value == null && value1 == null)
            {
                return true;
            }

            if (value == null || value1 == null)
            {
                return false;
            }

            if (value1.Equals(value)) return true;

            return CheckProperty(propertyFlags, recurseComplexTypeProperties, predicate, property, value1, value);
        }

        private static bool CheckProperty(BindingFlags propertyFlags, bool recurseComplexTypeProperties, Func<PropertyInfo, bool> predicate,
            PropertyInfo property, object value1, object value)
        {
            if (!property.PropertyType.CanUseForDb() && recurseComplexTypeProperties)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse

                if (!PropertiesEquals(property.PropertyType, value1, value, propertyFlags, recurseComplexTypeProperties,
                    predicate))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool ArraysEqual<T>(this T[] a1, T[] a2)
        {
            // also see Enumerable.SequenceEqual(a1, a2).
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            return !a1.Where((t, i) => !comparer.Equals(t, a2[i])).Any();
        }

        /// <summary>
        /// 比较两个实体属性值是否相等(不比较索引器)。
        /// </summary>
        /// <param name="thisObject"></param>
        /// <param name="obj">要比较的实体,可以是派生类型</param>
        /// <param name="propertyFlags">属性的一个或多个搜索执行方式</param>
        /// <param name="predicate">属性比较筛选器，如果不为空，必须满足删选器条件的属性才进行比较。</param>
        /// <param name="recurseComplexTypeProperties">是否递归复合类型属性。</param>
        public static bool PropertiesEquals(this object thisObject, object obj, BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Instance, bool recurseComplexTypeProperties = true, Func<PropertyInfo, bool> predicate = null)
        {
            if (thisObject == null && obj == null || ReferenceEquals(thisObject, obj))
            {
                return true;
            }

            if (thisObject == null || obj == null) return false;
            var declareType = thisObject.GetType();
            var type = obj.GetType();
            return declareType.GetTypeInfo().IsAssignableFrom(type) && PropertiesEquals(declareType, thisObject, obj, propertyFlags, recurseComplexTypeProperties, predicate);
        }

        public static bool ContainsAttributes(this PropertyInfo info, Type[] attributeTypes, bool inherit)
        {
            if (attributeTypes == null || attributeTypes.Length <= 0)
                throw new ArgumentException("the attributeTypes cant be null or empty array");
            foreach (var type in attributeTypes)
            {

                if (typeof(Attribute).GetTypeInfo().IsAssignableFrom(type))
                {
                    if (!info.GetCustomAttributes(type, inherit).Any())
                    {
                        return false;
                    }
                }
                else
                {
                    throw new ArgumentException(
                        $"the item of attributeTypes must be a type of {typeof(Attribute).AssemblyQualifiedName}");
                }
            }
            return true;

        }

        #endregion
    }
}
