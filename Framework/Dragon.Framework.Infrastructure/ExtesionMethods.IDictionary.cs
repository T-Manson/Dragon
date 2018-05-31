using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    partial class ExtensionMethods
    {
        /// <summary>
        /// 合并字典。
        /// </summary>
        /// <param name="instance">源字典。</param>
        /// <param name="toAdd">要合并的字典。</param>
        /// <param name="replaceExisting">如果为 <c>true</c>，将替换已存在的项，如果为 false, 跳过已存在的项.</param>
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> instance, IEnumerable<KeyValuePair<TKey, TValue>> toAdd, bool replaceExisting = true)
        {
            Guard.ArgumentNotNull(toAdd, nameof(toAdd));

            foreach (var pair in toAdd)
            {
                instance.Set(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// 仅在字典中不存在键时候插入键和值，如果存在不进行插入。
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key">要插入的键。</param>
        /// <param name="value">要插入的值。</param>
        /// <returns>如果插入了值，返回 true；否则，返回 false。</returns>
        public static bool AddIfNotExisting<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
            {
                return false;
            }
            dic.Add(key, value);
            {
                return true;
            }
        }

        public static TValue RemoveAndGet<TKey, TValue>(this IDictionary<TKey, TValue> instance, TKey key)
        {
            if (!instance.TryGetValue(key, out var result)) return default(TValue);
            return instance.Remove(key) ? result : default(TValue);
        }

        /// <summary>
        /// 获取字典中的指定键的值，如果指定的键不存在，则返回 <paramref name="valueFactory"/> 委托的返回值 。
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> instance, TKey key, Func<TValue> valueFactory)
        {
            Guard.ArgumentNotNull(valueFactory, nameof(valueFactory));
            return !instance.TryGetValue(key, out var result) ? valueFactory.Invoke() : result;
        }

        /// <summary>
        /// 获取字典中的指定键的值，如果指定的键不存在，则返回 <paramref name="defaultValue"/> 。
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> instance, TKey key, TValue defaultValue = default(TValue))
        {
            return !instance.TryGetValue(key, out var result) ? defaultValue : result;
        }

        /// <summary>
        /// 如果指定的键尚不存在，则将键/值对添加到字典中。
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> instance, TKey key, Func<TKey, TValue> valueFunc)
        {
            Guard.ArgumentNotNull(valueFunc, nameof(valueFunc));

            if (instance.TryGetValue(key, out var result)) return result;
            result = valueFunc(key);
            instance.Add(key, result);

            return result;
        }

        /// <summary>
        /// 如果指定的键尚不存在，则将键/值对添加到字典中。
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, object> instance, TKey key, Func<TKey, TValue> valueFunc)
        {
            Guard.ArgumentNotNull(valueFunc, nameof(valueFunc));

            if (instance.TryGetValue(key, out var result)) return (TValue)result;
            result = valueFunc(key);
            instance.Add(key, result);

            return (TValue)result;
        }

        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey attribute, TValue value, bool replaceExisting = true)
        {
            if (dic.ContainsKey(attribute))
            {
                if (replaceExisting)
                {
                    dic[attribute] = value;
                }
            }
            else
            {
                dic.Add(attribute, value);
            }
        }

        public static IDictionary<string, object> AttachPrefiex(this IDictionary<string, object> values, string prefix)
        {
            if (prefix.IsNullOrWhiteSpace())
            {
                return values;
            }

            var newValues = values.Select(kp => new KeyValuePair<string, object>($"{prefix}.{kp.Key}", kp.Value));
            values.Clear();
            foreach (var value in newValues)
            {
                values.Set(value.Key, value.Value);
            }
            return values;
        }

        public static TValue Get<TKey, TValue>(
                this IDictionary<TKey, TValue> dictionary,
                TKey key,
                TValue defaultValue = default(TValue))
        {
            return dictionary.TryGetValue(key, out var v) ? v : defaultValue;
        }

        public static TValue SafeGetValue<TKey, TValue>(
                this IDictionary<TKey, TValue> dictionary,
                TKey key,
                object dictionaryLock,
                Func<TValue> valueInitializer)
        {
            var found = dictionary.TryGetValue(key, out var value);
            if (found) return value;

            lock (dictionaryLock)
            {
                found = dictionary.TryGetValue(key, out value);
                if (found) return value;

                value = valueInitializer();

                dictionary.Add(key, value);
            }
            return value;
        }

        public static async Task<TValue> SafeGetValueAsync<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            object syncObject,
            ManualResetEventSlim syncLock,
            Func<TKey, Task<TValue>> valueFunc)
        {
            var found = dictionary.TryGetValue(key, out var value);
            if (found)
            {
                return value;
            }

            lock (syncObject)
            {
                syncLock.Wait();
                syncLock.Reset();
            }
            found = dictionary.TryGetValue(key, out value);

            try
            {
                if (!found)
                {
                    value = await valueFunc(key).ConfigureAwait(false);
                    dictionary.Add(key, value);
                }
            }
            finally
            {
                syncLock.Set();
            }

            return value;
        }
    }
}
