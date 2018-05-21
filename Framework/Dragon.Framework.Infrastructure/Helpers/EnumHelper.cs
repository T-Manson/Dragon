using System;

namespace Dragon.Framework.Infrastructure.Helpers
{
    /// <summary>
    /// 枚举工具类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 尝试转成指定的枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">需要转成枚举的值</param>
        /// <param name="returnEnum">返回值对应的枚举</param>
        /// <returns></returns>
        public static bool TryParse<T>(object value, out T returnEnum) where T : struct
        {
            returnEnum = default(T);
            if (value == null) return false;

            // 必须判断value是否为string类型，否则会直接根据枚举中定义的属性名匹配。
            // https://github.com/dotnet/coreclr/blob/dev/release/2.0.0/src/mscorlib/src/System/RtType.cs#L3615
            if (!(value is string) && !Enum.IsDefined(typeof(T), value)) return false;

            // 转换主要方法
            if (!Enum.TryParse(value.ToString(), out returnEnum)) return false;
            return true;
        }
    }
}
