using System;
using System.Security;
using System.Text.RegularExpressions;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static partial class ExtensionMethods
    {
        /// <summary>
        /// 自动转换为正则表达式内可直接使用的字符串（自动插入转义字符）。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string EscapeForRegex(this string instance)
        {
            return instance.IsNullOrWhiteSpace() ? instance : Regex.Escape(instance);
        }

        /// <summary>
        /// 大小写敏感比较。
        /// </summary>
        public static bool CaseSensitiveEquals(this string instance, string comparing)
        {
            if (instance == null && comparing == null)
            {
                return true;
            }
            if (instance != null && comparing == null || instance == null)
            {
                return false;
            }
            return string.CompareOrdinal(instance, comparing) == 0;
        }

        /// <summary>
        /// 大小写忽略比较。
        /// </summary>
        public static bool CaseInsensitiveEquals(this string instance, string comparing)
        {
            if (instance == null && comparing == null)
            {
                return true;
            }
            if (instance != null && comparing == null || instance == null)
            {
                return false;
            }
            return instance.Equals(comparing, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 指示指定的 <see cref="System.String"/> 对象是 null 还是 System.String.Empty 字符串。
        /// </summary>
        public static bool IsNullOrEmpty(this string data)
        {
            return string.IsNullOrEmpty(data);
        }

        /// <summary>
        /// 指示指定的字符串是 null、空还是仅由空白字符组成。
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string data)
        {
               return string.IsNullOrWhiteSpace(data);
        }

        /// <summary>
        /// 	如果指定的 <see cref="System.String"/> 对象字符串是 null、空还是仅由空白字符组成则返回默认值。
        /// </summary>
        public static string IfNullOrWhiteSpace(this string value, string defaultValue)
        {
            return !value.IsNullOrWhiteSpace() ? value : defaultValue;
        }

        /// <summary>
        /// 	如果指定的 <see cref="System.String"/> 对象是 null 或 System.Empty 字符串则返回默认值。
        /// </summary>
        public static string IfNullOrEmpty(this string value, string defaultValue)
        {
            return !value.IsNullOrEmpty() ? value : defaultValue;
        }


        /// <summary>
        /// 获取一个值，指示当前字符串是否是 IPV4地址 ( XXX.XXX.XXX.XXX )。
        /// </summary>
        /// <param name="input"></param>
        public static bool IsIpv4Address(this string input)
        {
            return !input.IsNullOrWhiteSpace() && Regex.IsMatch(input, "^(((2[0-4]\\d)|(25[0-5]))|(1\\d{2})|([1-9]\\d)|(\\d))[.](((2[0-4]\\d)|(25[0-5]))|(1\\d{2})|([1-9]\\d)|(\\d))[.](((2[0-4]\\d)|(25[0-5]))|(1\\d{2})|([1-9]\\d)|(\\d))[.](((2[0-4]\\d)|(25[0-5]))|(1\\d{2})|([1-9]\\d)|(\\d))$");
        }

        public static SecureString ToSecurityString(this string input)
        {
            var pChar = input.ToCharArray();

            var ss = new SecureString();

            foreach (var c in pChar)
            {
                ss.AppendChar(c);
            }
            return ss;
        }
    }
}
