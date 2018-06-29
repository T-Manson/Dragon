using Dragon.Framework.Data.Dapper.Common.Enums;
using Dragon.Framework.Infrastructure;

namespace Dragon.Framework.Data.Dapper.Query
{
    /// <summary>
    /// 表示多个聚合条件的查询。
    /// </summary>
    public class CombinedQueryFilter : QueryFilter
    {
        /// <summary>
        /// 条件1
        /// </summary>
        public QueryFilter Filter1 { get; }

        /// <summary>
        /// 条件2
        /// </summary>
        public QueryFilter Filter2 { get; }

        /// <summary>
        /// 表示查询过滤器中的字段逻辑组合方式。
        /// </summary>
        public BooleanClause Clause { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filter1">条件1</param>
        /// <param name="filter2">条件2</param>
        /// <param name="clause">查询过滤器中的字段逻辑组合方式</param>
        public CombinedQueryFilter(QueryFilter filter1, QueryFilter filter2, BooleanClause clause)
        {
            Guard.ArgumentNotNull(filter1, nameof(filter1));
            Guard.ArgumentNotNull(filter2, nameof(filter2));

            Filter1 = filter1;
            Filter2 = filter2;
            Clause = clause;
        }
    }
}
