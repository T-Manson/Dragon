using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dragon.Framework.Infrastructure.Helpers;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 守护类
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// 判断（路径）参数中是否包含非法字符。
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="argumentName"></param>
        [System.Diagnostics.DebuggerHidden]
        public static void ArgumentContainsInvalidPathChars(string argument, string argumentName)
        {
            if (argument.IsNullOrWhiteSpace())
            {
                return;
            }
            var invliadChars = Path.GetInvalidPathChars();

            if (argument.Any(c => invliadChars.Contains(c)))
            {
                throw new ArgumentException($"@The provided string argument {argumentName} contains invalid path character.");
            }
        }

        /// <summary>
        /// 当条件不满足时抛出异常。
        /// </summary>
        /// <param name="condition">要测试的条件。</param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="message">异常消息。</param>
        [System.Diagnostics.DebuggerHidden]
        public static void ArgumentCondition(bool condition, string message, string paramName = null)
        {
            if (condition) return;
            var ex = paramName.IsNullOrWhiteSpace() ? new ArgumentException(message) : new ArgumentException(message, paramName);
            throw ex;
        }


        [System.Diagnostics.DebuggerHidden]
        public static void ArgumentIsUri(string argument, string argumentName, UriKind kind = UriKind.RelativeOrAbsolute)
        {
            if (argument.IsNullOrWhiteSpace())
                throw new ArgumentException($@"The provided string argument {argumentName} must  be uri.", argumentName);

            if (Uri.IsWellFormedUriString(Uri.EscapeUriString(argument), kind)) return;

            throw new ArgumentException($@"The provided string argument {argumentName} must  be uri.", argumentName);
        }
        [System.Diagnostics.DebuggerHidden]
        public static void AbsolutePhysicalPath(string argument, string argumentName)
        {
            if (argument.IsNullOrWhiteSpace())
            {
                throw new ArgumentException(
                    $@"The provided string argument {argumentName} must  be absolute physical path.", argumentName);
            }
            if (!Path.IsPathRooted(argument))
            {
                throw new ArgumentException(
                    $@"The provided string argument {argumentName} must  be absolute physical path.", argumentName);
            }
        }

        /// <summary>
        /// 当参数不是相对路径（包括文件系统路径和 Uri）是抛出异常。
        /// </summary>
        /// <param name="argument">参数。</param>
        /// <param name="argumentName">参数名。</param>
        [System.Diagnostics.DebuggerHidden]
        public static void ArgumentIsRelativePath(string argument, string argumentName)
        {
            if (argument.IsNullOrWhiteSpace())
            {
                throw new ArgumentException($@"The provided string argument {argumentName} must  be relative path.", argumentName);
            }
            ArgumentContainsInvalidPathChars(argument, argumentName);
            var virtualPath = argumentName.Replace(@"\", @"/");
            if (Uri.IsWellFormedUriString(Uri.EscapeUriString(virtualPath), UriKind.Absolute))
            {
                throw new ArgumentException($@"The provided string argument {argumentName} must  be relative path.", argumentName);
            }
            var path = argument.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (Path.IsPathRooted(path))
            {
                throw new ArgumentException($@"The provided string argument {argumentName} must  be relative path.", argumentName);
            }
        }

        [System.Diagnostics.DebuggerHidden]
        public static void ArgumentNullOrWhiteSpaceString(string argumentValue, string argumentName)
        {
            ArgumentNotNullOrEmptyString(argumentValue, argumentName, true);
        }

        [System.Diagnostics.DebuggerHidden]
        public static void ArgumentNotNullOrEmptyString(string argumentValue, string argumentName)
        {
            ArgumentNotNullOrEmptyString(argumentValue, argumentName, false);
        }

        private static void ArgumentNotNullOrEmptyString(string argumentValue, string argumentName, bool trimString)
        {
            if (trimString && argumentValue.IsNullOrWhiteSpace() || !trimString && argumentValue.IsNullOrEmpty())
            {
                throw new ArgumentException($@"The provided string argument {argumentName} must not be empty.");
            }
        }

        [System.Diagnostics.DebuggerHidden]
        public static void ArgumentNotNullOrEmptyArray<T>(IEnumerable<T> argumentValue, string argumentName)
        {
            if (argumentValue == null || !argumentValue.Any())
            {
                throw new ArgumentException(
                    $@"The provided array argument {argumentName} must not be null or empty array.");
            }
        }

        /// <summary>
        /// Checks an argument to ensure it isn't null
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        [System.Diagnostics.DebuggerHidden]
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Checks an Enum argument to ensure that its value is defined by the specified Enum type.
        /// </summary>
        /// <param name="enumType">The Enum type the value should correspond to.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="argumentName">The name of the argument holding the value.</param>
        [System.Diagnostics.DebuggerHidden]
        public static void EnumValueIsDefined(Type enumType, object value, string argumentName)
        {
            if (!enumType.HasAttribute<FlagsAttribute>())
            {
                if (!Enum.IsDefined(enumType, value))
                    throw new ArgumentException(
                        $"The value of the argument {argumentName} provided for the enumeration {enumType} is invalid.");
            }
            else
            {
                try
                {
                    ExpressionHelper.MakeConvertLambda(enumType).Compile().DynamicInvoke(value);
                }
                catch (TargetInvocationException)
                {
                    throw new ArgumentException(
                        $"The value of the argument {argumentName} provided for the enumeration {enumType} is invalid.");
                }
            }
        }

        /// <summary>
        ///判断类型是否能够从提供的类型分配实例（assignee 是否继承于 providedType）。
        /// </summary>
        /// <param name="assignee">参数类型。</param>
        /// <param name="providedType">要从中分配实例的类型，通常为接口或基类。</param>
        /// <param name="argumentName">参数名称。</param>
        [System.Diagnostics.DebuggerHidden]
        public static void TypeIsAssignableFromType(Type assignee, Type providedType, string argumentName)
        {
            if (!providedType.GetTypeInfo().IsAssignableFrom(assignee))
                throw new ArgumentException($"The provided type {assignee} is not compatible with {providedType}.", argumentName);
        }

        [System.Diagnostics.DebuggerHidden]
        public static void DateTimeKind(DateTimeKind kind, DateTime value, string argumentName)
        {
            if (value.Kind != kind)
            {
                throw new ArgumentException(
                    $"The datetime kind of the argument '{argumentName}' must be {kind.ToString()}.");
            }
        }

        [System.Diagnostics.DebuggerHidden]
        public static void DateTimeKind(DateTimeKind kind, DateTime? value, string argumentName)
        {
            if (value.HasValue && value.Value.Kind != kind)
            {
                throw new ArgumentException(
                    $"The datetime kind of the argument '{argumentName}' must be {kind.ToString()}.");
            }
        }

        [System.Diagnostics.DebuggerHidden]
        public static void InSecondRange(int data, string argumentName)
        {
            if (data > 59 || data < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, "second must between 0 an 59");
            }
        }

        [System.Diagnostics.DebuggerHidden]
        public static void InMonthRange(int data, string argumentName)
        {
            if (data > 12 || data < 1)
            {
                throw new ArgumentOutOfRangeException(argumentName, "month must between 1 an 59");
            }
        }

        [System.Diagnostics.DebuggerHidden]
        public static void InMinuteRange(int data, string argumentName)
        {
            if (data > 59 || data < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, "minute must between 0 an 59");
            }
        }

        [System.Diagnostics.DebuggerHidden]
        public static void InHourRange(int data, string argumentName)
        {
            if (data > 23 || data < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, "minute must between 0 an 23");
            }
        }

        [System.Diagnostics.DebuggerHidden]
        public static void InMonthDayRange(int data, string argumentName)
        {
            if (data > 31 || data < 1)
            {
                throw new ArgumentOutOfRangeException(argumentName, "month day must between 1 an 31");
            }
        }
    }
}
