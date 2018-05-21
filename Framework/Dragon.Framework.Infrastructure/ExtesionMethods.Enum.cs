using System;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static partial class ExtensionMethods
    {
        /// <summary>
        /// 获取枚举属性对应的描述
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumValue)
        {
            return EnumDescriptionAttribute.GetDescription(enumValue);
        }
    }
}
