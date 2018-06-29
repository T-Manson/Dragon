using Dragon.Framework.Data.Dapper.Common.Enums;
using Dragon.Framework.Infrastructure;

namespace Dragon.Framework.Data.Dapper.Common
{
    /// <summary>
    /// 字段筛选条件。
    /// </summary>
    public class FieldPredicate
    {
        /// <summary>
        /// 获取或设置字段名。
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// 获取或设置二元操作的类型。
        /// </summary>
        public BinaryOperation Operation { get; }

        /// <summary>
        /// 获取或设置运算用的值（可以为空）。
        /// </summary>
        public object OperationValue { get; }

        /// <summary>
        /// 创建 <see cref="FieldPredicate"/> 类的新实例。
        /// </summary>
        /// <param name="fieldName">参与二元运算的字段名。</param>
        /// <param name="binaryOperation">二元运算操作符。</param>
        /// <param name="value">用于运算的值。</param>
        public FieldPredicate(string fieldName, BinaryOperation binaryOperation, object value)
        {
            Guard.ArgumentNullOrWhiteSpaceString(fieldName, nameof(fieldName));

            FieldName = MappingStrategyParser.Parse(fieldName);
            Operation = binaryOperation;
            OperationValue = value;
        }
    }
}
