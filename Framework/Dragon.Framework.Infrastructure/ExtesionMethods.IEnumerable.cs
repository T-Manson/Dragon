using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static partial class ExtensionMethods
    {

        #region TableString

        /// <summary>
        /// 生成 ascii 表格。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">要生成表格的元素。</param>
        /// <param name="columnHeaders">列名。</param>
        /// <param name="valueSelectors">每一行数据取出的列数据。</param>
        /// <returns></returns>
        public static string ToStringTable<T>(this IEnumerable<T> values, string[] columnHeaders, params Func<T, object>[] valueSelectors)
        {
            return ToStringTable(values.ToArray(), columnHeaders, valueSelectors);
        }

        private static string ToStringTable<T>(this T[] values, string[] columnHeaders, params Func<T, object>[] valueSelectors)
        {
            Debug.Assert(columnHeaders.Length == valueSelectors.Length);

            var arrValues = new string[values.Length + 1, valueSelectors.Length];

            // Fill headers
            for (var colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
            {
                arrValues[0, colIndex] = columnHeaders[colIndex];
            }

            // Fill table rows
            for (var rowIndex = 1; rowIndex < arrValues.GetLength(0); rowIndex++)
            {
                for (var colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    var value = valueSelectors[colIndex].Invoke(values[rowIndex - 1]);

                    arrValues[rowIndex, colIndex] = value?.ToString() ?? "null";
                }
            }

            return ToStringTable(arrValues);
        }

        private static string ToStringTable(this string[,] arrValues)
        {
            var maxColumnsWidth = GetMaxColumnsWidth(arrValues);
            var maxRowHeight = GetMaxRowHeight(arrValues);
            var headerSpliter = new string('-', maxColumnsWidth.Sum(i => i + 3) - 1);

            var sb = new StringBuilder();
            for (var rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
            {
                WriteRow(arrValues, maxColumnsWidth, headerSpliter, sb, rowIndex, maxRowHeight[rowIndex]);
            }

            return sb.ToString();
        }

        private static void WriteRow(string[,] arrValues, int[] maxColumnsWidth, string headerSpliter, StringBuilder sb, int rowIndex, int rowHeight)
        {
            for (var i = 0; i < rowHeight; i++)
            {
                for (var colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    // Print cell
                    var cell = arrValues[rowIndex, colIndex];
                    var splitedValues = cell.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    cell = (splitedValues.Length > i) ? splitedValues[i] : " ";
                    cell = cell.PadRight(maxColumnsWidth[colIndex]);
                    sb.Append(" | ");
                    sb.Append(cell);
                }

                // Print end of line
                sb.Append(" | ");
                sb.AppendLine();

                // Print splitter
                if (rowIndex == 0)
                {
                    sb.AppendFormat(" |{0}| ", headerSpliter);
                    sb.AppendLine();
                }
            }
        }

        private static int[] GetMaxRowHeight(string[,] arrValues)
        {
            var maxRowHeight = new int[arrValues.GetLength(0)];
            for (var rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
            {
                for (var colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    var newLenght = arrValues[rowIndex, colIndex].Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length;
                    maxRowHeight[rowIndex] = Math.Max(maxRowHeight[rowIndex], newLenght);
                }
            }
            return maxRowHeight;
        }

        private static int[] GetMaxColumnsWidth(string[,] arrValues)
        {
            var maxColumnsWidth = new int[arrValues.GetLength(1)];
            for (var colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
            {
                for (var rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
                {
                    var values = arrValues[rowIndex, colIndex].Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    var newLength = values.Length == 0 ? 0 : values.Max(s => s?.Length ?? 0);
                    var oldLength = maxColumnsWidth[colIndex];

                    maxColumnsWidth[colIndex] = Math.Max(newLength, oldLength);
                }
            }

            return maxColumnsWidth;
        }

        public static string ToStringTable<T>(this IEnumerable<T> values, params Expression<Func<T, object>>[] valueSelectors)
        {
            var headers = valueSelectors.Select(func => GetProperty(func)?.Name ?? string.Empty).ToArray();
            var selectors = valueSelectors.Select(exp => exp.Compile()).ToArray();
            return ToStringTable(values, headers, selectors);
        }

        private static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expresstion)
        {
            if ((expresstion.Body as UnaryExpression)?.Operand is MemberExpression)
            {
                return (((UnaryExpression) expresstion.Body).Operand as MemberExpression)?.Member as PropertyInfo;
            }

            return (expresstion.Body as MemberExpression)?.Member as PropertyInfo;
        }

        #endregion

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ObservableCollection<T>(enumerable);
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }

        public static IEnumerable<TEntity> Pagination<TEntity>(this IEnumerable<TEntity> entities, int pageIndex, int pageSize)
            where TEntity : class
        {
            var skipQuery = pageIndex > 0 ? entities.Skip(pageIndex * pageSize) : entities;
            var entries = skipQuery.Take(pageSize);
            return entries;
        }

        public static IQueryable<TEntity> Pagination<TEntity>(this IQueryable<TEntity> entities, int pageIndex, int pageSize)
            where TEntity : class
        {
            var skipQuery = pageIndex > 0 ? entities.Skip(pageIndex * pageSize) : entities;
            var entries = skipQuery.Take(pageSize);
            return entries;
        }

        public static void For<T>(this IEnumerable<T> enums, Action<int, T> action)
        {
            enums.For((index, item) => { action(index, item); return true; });
        }

        public static void ForEach<T>(this IEnumerable<T> enums, Action<T> action)
        {
            enums.ForEach(item => { action(item); return true; });
        }

        public static void For<T>(this IEnumerable<T> enums, Func<int, T, bool> action)
        {
            if (enums == null || action == null)
            {
                return;
            }
            var i = 0;
            foreach (var item in enums)
            {
                if (!action(i, item))
                {
                    break;
                }
                i++;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enums, Func<T, bool> action)
        {
            if (enums == null || action == null)
            {
                return;
            }
            var array = enums.ToArray();
            foreach (var item in array)
            {
                if (!action(item))
                {
                    break;
                }
            }
        }

        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            return source == null || !source.GetEnumerator().MoveNext();
        }

        public static IEnumerable<TSource> Distinct<TSource, TProperty>(this IEnumerable<TSource> source, Expression<Func<TSource, TProperty>> comparerProperty)
        {
            return source.Distinct(new PropertyComparer<TSource>(comparerProperty.GetMemberName()));
        }

        public static IEnumerable<TSource> Intersect<TSource, TProperty>(this IEnumerable<TSource> source, IEnumerable<TSource> second, Expression<Func<TSource, TProperty>> comparerProperty)
        {
            return source.Intersect(second, new PropertyComparer<TSource>(comparerProperty.GetMemberName()));
        }

        public static IEnumerable<TSource> Except<TSource, TProperty>(this IEnumerable<TSource> source, IEnumerable<TSource> second, Expression<Func<TSource, TProperty>> comparerProperty)
        {
            return source.Except(second, new PropertyComparer<TSource>(comparerProperty.GetMemberName()));
        }

        public static string ToArrayString<T>(this IEnumerable<T> array, string separator = ",")
        {
            return string.Join(separator, array.Select(t => t.ToString()));
        }

        public static string ToArrayString<T>(this IEnumerable<T> array, Func<T, string> reduce, string separator = ",")
        {
            Guard.ArgumentNotNull(reduce, nameof(reduce));
            return string.Join(separator, array.Select(reduce));
        }
    }
}