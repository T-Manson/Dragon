using Dragon.Framework.Data.Dapper.Common.Enums;
using Dragon.Framework.Infrastructure;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dragon.Framework.Data.Dapper.Common
{
    /// <summary>
    /// 表示查询的排序选项。
    /// </summary>
    public class SortOptions
    {
        /// <summary>
        /// 字段排序规则集合
        /// </summary>
        private readonly ConcurrentDictionary<string, SortOrder> _fieldSortDictionary;

        /// <summary>
        /// 字段排序规则集合
        /// </summary>
        public ReadOnlyDictionary<string, SortOrder> FieldSortDictionary => new ReadOnlyDictionary<string, SortOrder>(_fieldSortDictionary);

        /// <summary>
        /// 创建 <see cref="SortOptions"/> 的新实例。
        /// </summary>
        /// <param name="fieldName">排序字段的名称。</param>
        /// <param name="sort">排序方式。</param>
        public SortOptions(string fieldName, SortOrder sort = SortOrder.Ascending)
        {
            Guard.ArgumentNullOrWhiteSpaceString(fieldName, nameof(fieldName));

            _fieldSortDictionary = new ConcurrentDictionary<string, SortOrder>();
            _fieldSortDictionary.TryAdd(fieldName, sort);
        }

        /// <summary>
        /// 为当前排序选项添加排序字段（字段排序优先规则按添加的顺序）。
        /// </summary>
        /// <param name="fieldName">要添加的字段名。</param>
        /// <param name="sort">排序方式。</param>
        /// <returns></returns>
        public SortOptions AddField(string fieldName, SortOrder sort = SortOrder.Ascending)
        {
            Guard.ArgumentNullOrWhiteSpaceString(fieldName, nameof(fieldName));

            if (!_fieldSortDictionary.ContainsKey(fieldName))
                _fieldSortDictionary.TryAdd(fieldName, sort);
            return this;
        }

        /// <summary>
        /// 获取用于排序的字段。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetFields()
        {
            return _fieldSortDictionary.Keys;
        }
    }
}
