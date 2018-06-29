using Dragon.Framework.Data.Dapper.Common;
using Dragon.Framework.Data.Dapper.Common.Enums;
using Dragon.Framework.Infrastructure;
using System.Collections;
using System.Collections.Generic;

namespace Dragon.Framework.Data.Dapper.Query
{
    /// <summary>
    /// 简单的查询筛选
    /// </summary>
    public class SingleQueryFilter : QueryFilter, IEnumerable<FieldPredicate>
    {
        /// <summary>
        /// 条件集合
        /// </summary>
        private readonly HashSet<FieldPredicate> _predicateSet = null;

        /// <summary>
        /// 字段比较规则
        /// </summary>
        private static readonly IEqualityComparer<FieldPredicate> FieldPredicateComparer;

        /// <summary>
        /// 表示查询过滤器中的字段逻辑组合方式。
        /// </summary>
        public BooleanClause Clause { get; set; }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty => _predicateSet.Count == 0;

        /// <summary>
        /// 类型构造
        /// </summary>
        static SingleQueryFilter()
        {
            FieldPredicateComparer = new GenericEqualityComparer<FieldPredicate>((f1, f2)
                => f1.FieldName.CaseInsensitiveEquals(f2.FieldName) &&
                f1.Operation.Equals(f2.Operation) &&
                f1.OperationValue.SafeEquals(f2.OperationValue));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SingleQueryFilter(BooleanClause clause = BooleanClause.And)
        {
            Clause = clause;
            _predicateSet = new HashSet<FieldPredicate>(FieldPredicateComparer);
        }

        #region predicate helpers

        /// <summary>
        /// 添加“等于”筛选。
        /// </summary>
        /// <param name="fieldName">要删选的字段名。</param>
        /// <param name="value">要删选的值。</param>
        /// <returns></returns>
        public SingleQueryFilter AddEqual(string fieldName, object value)
        {
            _predicateSet.Add(new FieldPredicate(fieldName, BinaryOperation.Equal, value));
            return this;
        }

        /// <summary>
        /// 添加“不等于”筛选。
        /// </summary>
        /// <param name="fieldName">要删选的字段名。</param>
        /// <param name="value">要删选的值。</param>
        /// <returns></returns>
        public SingleQueryFilter AddNotEqual(string fieldName, object value)
        {
            _predicateSet.Add(new FieldPredicate(fieldName, BinaryOperation.NotEqual, value));
            return this;
        }

        /// <summary>
        /// 添加“大于”筛选。
        /// </summary>
        /// <param name="fieldName">要删选的字段名。</param>
        /// <param name="value">要删选的值。</param>
        /// <returns></returns>
        public SingleQueryFilter AddGreater(string fieldName, object value)
        {
            _predicateSet.Add(new FieldPredicate(fieldName, BinaryOperation.Greater, value));
            return this;
        }

        /// <summary>
        /// 添加“大于等于”筛选。
        /// </summary>
        /// <param name="fieldName">要删选的字段名。</param>
        /// <param name="value">要删选的值。</param>
        /// <returns></returns>
        public SingleQueryFilter AddGreaterOrEqual(string fieldName, object value)
        {
            _predicateSet.Add(new FieldPredicate(fieldName, BinaryOperation.GreaterOrEqual, value));
            return this;
        }

        /// <summary>
        /// 添加“小于”筛选。
        /// </summary>
        /// <param name="fieldName">要删选的字段名。</param>
        /// <param name="value">要删选的值。</param>
        /// <returns></returns>
        public SingleQueryFilter AddLess(string fieldName, object value)
        {
            _predicateSet.Add(new FieldPredicate(fieldName, BinaryOperation.Less, value));
            return this;
        }

        /// <summary>
        /// 添加“小于等于”筛选。
        /// </summary>
        /// <param name="fieldName">要删选的字段名。</param>
        /// <param name="value">要删选的值。</param>
        /// <returns></returns>
        public SingleQueryFilter AddLessOrEqual(string fieldName, object value)
        {
            _predicateSet.Add(new FieldPredicate(fieldName, BinaryOperation.LessOrEqual, value));
            return this;
        }

        #endregion

        /// <summary>
        /// 添加筛选条件。
        /// </summary>
        /// <param name="fieldName">用于筛选的字段。</param>
        /// <param name="operation">二元操作符。</param>
        /// <param name="value">用于筛选的值。</param>
        public void AddPredicate(string fieldName, BinaryOperation operation, object value)
        {
            _predicateSet.Add(new FieldPredicate(fieldName, operation, value));
        }

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        IEnumerator<FieldPredicate> IEnumerable<FieldPredicate>.GetEnumerator()
        {
            return _predicateSet.GetEnumerator();
        }

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _predicateSet.GetEnumerator();
        }
    }
}
