using Dragon.Framework.Data.Dapper.Common;
using Dragon.Framework.Data.Dapper.Common.Enums;
using Dragon.Framework.Data.Dapper.Conventions;
using System;
using System.Reflection;

namespace Dragon.Framework.Data.Dapper.Metadata
{
    /// <summary>
    /// 表示 Dapper 属性字段的元数据。
    /// </summary>
    public class DapperFieldMetadata
    {
        /// <summary>
        /// 属性字段
        /// </summary>
        public PropertyInfo Field { get; }

        /// <summary>
        /// 属性字段名称（根据实体字段名称与映射策略计算出）
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 自增
        /// </summary>
        public bool AutoGeneration { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 忽略
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="field">字段</param>
        internal DapperFieldMetadata(PropertyInfo field)
        {
            Field = field;
            Name = MappingStrategyParser.Parse(field.Name);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="convention">属性约定</param>
        internal DapperFieldMetadata(PropertyInfo field, PropertyConvention convention)
        {
            Field = field;
            Name = MappingStrategyParser.Parse(field.Name);

            var rule = convention.DbColumnRule;
            var valueOf = new Func<DbColumnRule, DbColumnRule, bool>((x, y) => (x & y) == y);
            AutoGeneration = valueOf(rule, DbColumnRule.AutoGeneration);
            IsKey = valueOf(rule, DbColumnRule.Key);
            Ignore = valueOf(rule, DbColumnRule.Ignore);
        }
    }
}
