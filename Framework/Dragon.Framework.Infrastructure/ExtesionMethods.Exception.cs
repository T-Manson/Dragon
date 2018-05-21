using System;
using System.Collections.Generic;
using System.Linq;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static partial class ExtensionMethods
    {
        public static Exception GetOriginalException(this Exception exception)
        {
            return exception.InnerException == null ? exception : exception.InnerException.GetOriginalException();
        }

        public static TException GetOriginalException<TException>(this Exception exception)
            where TException : Exception
        {
            var ex = exception as TException;
            if (exception == null)
            {
                return null;
            }

            return ex ?? exception.InnerException.GetOriginalException<TException>();
        }

        public static IEnumerable<string> Messages(this Exception exception)
        {
            return exception != null ?
                    new List<string>(exception.InnerException.Messages()) { exception.Message } : Enumerable.Empty<string>();
        }

        public static IEnumerable<Exception> Exceptions(this Exception exception)
        {
            return exception != null ?
                    new List<Exception>(exception.InnerException.Exceptions()) { exception } : Enumerable.Empty<Exception>();
        }

        /// <summary>
        /// 抛出无法处理的异常，例如堆栈溢出、算术溢出等）。
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static void ThrowIfNecessary(this Exception exception)
        {
            Guard.ArgumentCondition(exception is OutOfMemoryException || exception is OverflowException || exception is InvalidCastException,
                exception.StackTrace);
        }
    }
}
