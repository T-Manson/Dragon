using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static partial class ExtensionMethods
    {
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 获取日期的最小时间表示形式(00:00:00)
        /// </summary>
        public static DateTimeOffset ToMinTimeDate(this DateTimeOffset date)
        {
            var result = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Offset);
            return result;
        }

        /// <summary>
        /// 获取日期的最小时间表示形式(00:00:00)
        /// </summary>
        public static DateTime ToMinTimeDate(this DateTime date)
        {
            var result = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind);
            return result;
        }

        /// <summary>
        /// 获取日期的最大时间表示形式(23:59:59)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ToMaxTimeDate(this DateTime date)
        {
            var result = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind);

            return result;
        }

        /// <summary>
        /// 返回自 1970 年 1 月 1 日 00:00:00 GMT 以来此 <see cref="DateTime"/> 对象表示的毫秒数。 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long GetUnixTime(this DateTime date)
        {
            var diff = date.ToUniversalTime() - Jan1St1970;
            return (long)Math.Floor(diff.TotalMilliseconds);
        }

        /// <summary>
        /// 获取日期的最大时间表示形式(23:59:59)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTimeOffset ToMaxTimeDate(this DateTimeOffset date)
        {
            var result = new DateTimeOffset(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Offset);

            return result;
        }

        /// <summary>
        /// 获取当前时间所在周的日期范围。
        /// </summary>
        public static RangeOfDateTime CurrentWeek(this DateTime dateTime)
        {
            var start = dateTime.AddDays(1 - int.Parse(dateTime.DayOfWeek.ToString("d"))).ToMinTimeDate();
            var end = start.AddDays(6).ToMaxTimeDate();
            return new RangeOfDateTime(start, end);
        }

        /// <summary>
        /// 获取当前时间所在月的日期范围。
        /// </summary>
        public static RangeOfDateTime CurrentMonth(this DateTime dateTime)
        {
            var start = dateTime.AddDays(1 - dateTime.Day).ToMinTimeDate();
            var end = start.AddMonths(1).AddDays(-1).ToMaxTimeDate();
            return new RangeOfDateTime(start, end);
        }

        /// <summary>
        /// 获取当前时间所在季度的日期范围。
        /// </summary>
        public static RangeOfDateTime CurrentQuarter(this DateTime dateTime)
        {
            var start = dateTime.AddMonths(0 - (dateTime.Month - 1) % 3).AddDays(1 - dateTime.Day).ToMinTimeDate();
            var end = start.AddMonths(3).AddDays(-1).ToMaxTimeDate();
            return new RangeOfDateTime(start, end);
        }

        /// <summary>
        /// 获取当前日期处于一年之中的哪个季度。
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int QuarterOfYear(this DateTime dateTime)
        {
            return dateTime.Month / 4 + 1;
        }

        public static DateTime ChangeKind(this DateTime date, DateTimeKind kind)
        {
            var result = new DateTime(date.Ticks, kind);
            return result;
        }

        public static DateTime? ChangeKind(this DateTime? date, DateTimeKind kind)
        {
            return date.HasValue ? new DateTime?(date.Value.ChangeKind(kind)) : null;
        }

        public static DateTime? ToUniversalTime(this DateTime? date)
        {
            return date.HasValue ? new DateTime?(date.Value.ToUniversalTime()) : null;
        }

        public static DateTime WithoutMillis(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
        }

        /// <summary>
        /// 四舍五入到指定的小数位。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals">要保留的精度（小数位数）</param>
        /// <returns></returns>
        public static decimal Precision(this decimal value, int decimals)
        {
            return value < 0 ? Math.Round(value + 5 / (decimal)Math.Pow(10, decimals + 1), decimals, MidpointRounding.AwayFromZero) : Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 四舍五入到指定的小数位。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals">要保留的精度（小数位数）</param>
        /// <returns></returns>
        public static double Precision(this double value, int decimals)
        {
            return value < 0 ? Math.Round(value + 5 / Math.Pow(10, decimals + 1), decimals, MidpointRounding.AwayFromZero) : Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 四舍五入到指定的小数位。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals">要保留的精度（小数位数）</param>
        /// <returns></returns>
        public static double Precision(this float value, int decimals)
        {
            return value < 0 ? Math.Round(value + 5 / Math.Pow(10, decimals + 1), decimals, MidpointRounding.AwayFromZero) : Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 判断是否为基数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool IsOdd(this int n)
        {
            return n % 2 != 0;
        }

        /// <summary>
        /// 判断是否为偶数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool IsEven(this int n)
        {
            return !n.IsOdd();
        }

        private static readonly ConcurrentDictionary<Enum, HashSet<Enum>> EnumFlagCache = new ConcurrentDictionary<Enum, HashSet<Enum>>();

        /// <summary>
        /// 获得枚举的位值（主要用于位枚举）。
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static IEnumerable<Enum> GetEnumBitValues(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var isFlagEnum = enumType.HasAttribute<FlagsAttribute>();
            var enumValues = EnumFlagCache.GetOrAdd(enumValue, v =>
            {
                var values = new HashSet<Enum>();
                if (isFlagEnum)
                {
                    foreach (var d in Enum.GetValues(enumType).Cast<Enum>())
                    {
                        if (AddValue(enumValue, d, values)) break;
                        FlagAdd(v, d, values);
                    }
                }
                else
                {
                    values.Add(v);
                }
                return values;
            });
            return enumValues;
        }

        private static bool AddValue(Enum enumValue, Enum d, HashSet<Enum> values)
        {
            if (Convert.ToInt32(d) != 0 || !enumValue.Equals(d)) return false;
            values.Add(d);
            return true;
        }

        private static void FlagAdd(Enum v, Enum d, HashSet<Enum> values)
        {
            if (v.HasFlag(d) && Convert.ToInt32(d) != 0)
                values.Add(d);
        }
    }
}
