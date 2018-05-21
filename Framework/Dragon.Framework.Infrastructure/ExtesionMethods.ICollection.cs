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
        public static void Replace<T>(this IList<T> collection, Func<T, bool> predicate, T item)
        {
            if (collection == null)
            {
                throw new InvalidOperationException("ReplaceItem 方法源序列为空。");
            }
            if (!collection.Contains(item))
            {
                try
                {
                    var oldItem = collection.First(predicate);
                    var index = collection.IndexOf(oldItem);
                    if (index >= 0)
                    {
                        collection[index] = item;
                    }
                }
                catch (InvalidOperationException)
                {
                    // do nothing
                }
            }
        }

        public static bool Remove<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            if (collection == null)
            {
                throw new InvalidOperationException("ReplaceItem 方法源序列为空。");
            }
            try
            {
                var item = collection.First(predicate);
                return collection.Remove(item);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
            {
                throw new InvalidOperationException("ReplaceItem 方法源序列为空。");
            }
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (!collection.Contains(item))
                    {
                        collection.Add(item);
                    }
                }
            }
        }
    }
}
