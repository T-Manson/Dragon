using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 枚举描述特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 枚举描述的资源名称。
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// 枚举描述的资源类型。
        /// </summary>
        public Type ResourceType { get; set; }

        /// <summary>
        /// 位枚举描述分隔符
        /// </summary>
        public static string FlagSplitCharacter { get; set; } = " | ";

        /// <summary>
        /// 是否缓存描述信息
        /// </summary>
        public static bool CacheDescription { get; set; } = true;

        /// <summary>
        /// 缓存枚举属性描述信息
        /// </summary>
        private static readonly ConcurrentDictionary<Enum, string> DescriptionCache = new ConcurrentDictionary<Enum, string>();

        #region Init

        public EnumDescriptionAttribute()
        {
            Description = string.Empty;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="defaultDescription">默认描述</param>
        public EnumDescriptionAttribute(string defaultDescription)
        {
            Description = defaultDescription;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resourceName">枚举描述的资源名称</param>
        /// <param name="resourceType">枚举描述的资源类型</param>
        public EnumDescriptionAttribute(string resourceName, Type resourceType)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");
            ResourceName = resourceName;
            ResourceType = resourceType;
        }

        #endregion

        /// <summary>
        /// 根据描述尝试获取枚举中对应的值
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="description">描述</param>
        /// <param name="enumValue">枚举值</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static bool TryParseEnumValue(Type enumType, string description, out object enumValue, bool ignoreCase = false)
        {
            enumValue = null;
            try
            {
                enumValue = ParseEnumValue(enumType, description, ignoreCase);
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据描述获取枚举中对应的值
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="description">描述</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static object ParseEnumValue(Type enumType, string description, bool ignoreCase = false)
        {
            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException($"{enumType.AssemblyQualifiedName} was not a enum type.");

            try
            {
                return Enum.Parse(enumType, description, ignoreCase);
            }
            catch (ArgumentException)
            {
                var values = GetEnumValues(enumType);
                object result = null;
                var descArray = description.Split(new[] { FlagSplitCharacter }, StringSplitOptions.RemoveEmptyEntries);
                var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

                foreach (var value in values)
                {
                    result = GetResult(enumType, description, descArray, comparer, value);
                    if (result != null) break;
                }

                if (result != null)
                {
                    return result;
                }
            }
            throw new ArgumentException($"The description {description} is not define on enum {enumType.Name}");
        }

        /// <summary>
        /// Get the enum description.
        /// </summary>
        /// <param name="enumValue">the enum value</param>
        /// <exception>TypeCompatibilityException</exception>
        public static string GetDescription(Enum enumValue)
        {
            Guard.ArgumentNotNull(enumValue, nameof(enumValue));
            return !CacheDescription
                ? GetDescriptionWithoutCache(enumValue.GetType(), enumValue)
                : DescriptionCache.GetOrAdd(enumValue, v => GetDescriptionWithoutCache(v.GetType(), v));
        }

        /// <summary>
        /// 获取描述信息
        /// </summary>
        /// <returns></returns>
        protected virtual string GetDescription()
        {
            if (ResourceType != null && ResourceName.IsNullOrWhiteSpace())
                return GetResourceDescription(ResourceName, ResourceType);
            return Description;
        }

        #region 公共方法

        /// <summary>
        /// 获取资源描述信息
        /// </summary>
        /// <param name="resourceName">枚举描述的资源名称</param>
        /// <param name="resourceType">枚举描述的资源类型</param>
        /// <returns></returns>
        private string GetResourceDescription(string resourceName, Type resourceType)
        {
            var description = string.Empty;
            var property = resourceType.GetTypeInfo().GetProperty(resourceName);
            var flag = false;
            if (!resourceType.GetTypeInfo().IsVisible || property == null || property.PropertyType != typeof(string))
            {
                flag = true;
            }
            else
            {
                var getMethod = property.GetGetMethod();
                if (getMethod == null || !getMethod.IsPublic || !getMethod.IsStatic)
                {
                    flag = true;
                }
            }
            if (!flag) description = (string)property.GetValue(null, null);
            return description;
        }

        /// <summary>
        /// 获取枚举中所有属性对应的值
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        private static IEnumerable<object> GetEnumValues(Type enumType)
        {
            return Enum.GetValues(enumType).Cast<object>();
        }

        /// <summary>
        /// 根据描述获取枚举中对应的值
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="description">描述</param>
        /// <param name="descArray">描述集合</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">枚举值</param>
        /// <returns></returns>
        private static object GetResult(Type enumType, string description, string[] descArray, StringComparer comparer, object value)
        {
            var desc = GetDescription((Enum)value);
            if (!enumType.HasAttribute<FlagsAttribute>() && comparer.Equals(desc, description))
            {
                return value;
            }

            object result = null;
            if (descArray.Contains(desc, comparer))
            {
                result = BinaryValues(enumType, ExpressionType.Or, result, value);
            }

            return result;
        }

        /// <summary>
        /// Get the enum description.
        /// </summary>
        /// <param name="enumType">the enum type</param>
        /// <param name="enumValue">the enum value</param>
        /// <exception>TypeCompatibilityException</exception>
        private static string GetDescriptionWithoutCache(Type enumType, object enumValue)
        {
            Guard.EnumValueIsDefined(enumType, enumValue, "enumValue");
            var isFlagEnum = enumType.HasAttribute<FlagsAttribute>();

            var names = Enum.GetNames(enumType);
            var currentName = new StringBuilder();

            foreach (var name in names)
            {
                var value = Enum.Parse(enumType, name, false);

                if (!isFlagEnum) //普通枚举。
                {
                    currentName = new StringBuilder(name);
                    var attribute = GetOrdinaryValue(value, enumValue, name, enumType);
                    if (attribute == null) continue;
                    return attribute.GetDescription().IfNullOrWhiteSpace(enumValue.ToString());
                }
                else //位枚举。
                {
                    var text = name;
                    var attribute = GetFlagValue(value, enumValue, text, enumType);
                    if (attribute == null) continue;
                    text = attribute.GetDescription().IfNullOrWhiteSpace(value.ToString());
                    currentName.Append($"{text}{FlagSplitCharacter}");
                }
            }

            if (isFlagEnum && currentName.Length > 0 && !FlagSplitCharacter.IsNullOrWhiteSpace())
            {
                currentName.Remove(currentName.Length - FlagSplitCharacter.Length, FlagSplitCharacter.Length);
            }
            return currentName.ToString();
        }

        private static EnumDescriptionAttribute GetFlagValue(object value, object enumValue, string name, Type enumType)
        {
            if (!((Enum)enumValue).HasFlag((Enum)value) && Convert.ToInt64(value) == 0 && Convert.ToInt64(enumValue) != 0) return null;
            var enumField = enumType.GetTypeInfo().GetField(name);
            return enumField.GetCustomAttribute<EnumDescriptionAttribute>();
        }

        private static EnumDescriptionAttribute GetOrdinaryValue(object value, object enumValue, string name, Type enumType)
        {
            if (!value.Equals(enumValue)) return null;
            var enumField = enumType.GetTypeInfo().GetField(name);
            return enumField.GetCustomAttribute<EnumDescriptionAttribute>();
        }

        private static object BinaryValues(Type enumType, ExpressionType type, object value1, object value2)
        {
            var x = Expression.Parameter(enumType, "x");
            var y = Expression.Parameter(enumType, "y");
            var underlyType = Enum.GetUnderlyingType(enumType);

            var left = Expression.Convert(x, underlyType);
            var right = Expression.Convert(y, underlyType);
            var and = Expression.MakeBinary(type, left, right);
            var process = Expression.Lambda(Expression.Convert(and, enumType), x, y);

            return process.Compile().DynamicInvoke(value1, value2);
        }

        #endregion
    }
}
