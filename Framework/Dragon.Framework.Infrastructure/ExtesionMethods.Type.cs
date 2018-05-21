using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static partial class ExtensionMethods
    {
        public static bool CanUseForDb(this Type type)
        {
            return type == typeof(string) ||
                 type == typeof(int) ||
                 type == typeof(long) ||
                 type == typeof(uint) ||
                 type == typeof(ulong) ||
                 type == typeof(float) ||
                 type == typeof(double) ||
                 type == typeof(Guid) ||
                 type == typeof(byte[]) ||
                 type == typeof(decimal) ||
                 type == typeof(char) ||
                 type == typeof(bool) ||
                 type == typeof(byte) ||
                 type == typeof(DateTime) ||
                 type == typeof(TimeSpan) ||
                 type == typeof(DateTimeOffset)||
                 type.GetTypeInfo().IsEnum ||
                 Nullable.GetUnderlyingType(type) != null && CanUseForDb(Nullable.GetUnderlyingType(type));
        }
        public static bool IsNullableType(this Type type, Type genericParameterType)
        {
            Guard.ArgumentNotNull(genericParameterType, nameof(genericParameterType));

            return genericParameterType == Nullable.GetUnderlyingType(type);
        }

        public static bool IsNullableEnum(this Type type)
        {
            return Nullable.GetUnderlyingType(type)?.GetTypeInfo().IsEnum ?? false;
        }

        public static bool HasAttribute<T>(this Type provider, bool inherit = false) where T : Attribute
        {
            return provider.GetTypeInfo().IsDefined(typeof(T), inherit);
        }

        public static IEnumerable<T> GetAttributes<T>(this Type provider, bool inherit = false) where T : Attribute
        {
            return provider.GetTypeInfo().GetCustomAttributes<T>(inherit);
        }

        public static T GetAttribute<T>(this Type provider, bool inherit = false) where T : Attribute
        {
            return provider.GetTypeInfo().GetCustomAttributes<T>(inherit).FirstOrDefault();
        }
    }
}
