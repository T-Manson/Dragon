using Dragon.Framework.Data.Dapper.Common.Enums;

namespace Dragon.Framework.Data.Dapper.Query
{
    /// <summary>
    /// 查询筛选器
    /// </summary>
    public abstract class QueryFilter
    {
        /// <summary>
        /// 通过 and 语义合并两个查询过滤器。
        /// </summary>
        public static QueryFilter CombineAnd(QueryFilter filter1, QueryFilter filter2)
        {
            return new CombinedQueryFilter(filter1, filter2, BooleanClause.And);
        }

        /// <summary>
        /// 通过 or 语义合并两个查询过滤器。
        /// </summary>
        public static QueryFilter CombineOr(QueryFilter filter1, QueryFilter filter2)
        {
            return new CombinedQueryFilter(filter1, filter2, BooleanClause.Or);
        }
    }
}
