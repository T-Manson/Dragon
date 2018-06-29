using Dragon.Framework.Data.Dapper.Common;
using Dragon.Framework.Data.Dapper.Conventions;
using Dragon.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dragon.Framework.Data.Dapper.Metadata
{
    /// <summary>
    /// 表示 Dapper 的元数据。
    /// </summary>
    public class DapperMetadata
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// 表名（根据实体名称与映射策略计算出）
        /// </summary>
        public string TableName { get; internal set; }

        /// <summary>
        /// 实体字段集合
        /// </summary>
        public List<DapperFieldMetadata> Fields { get; }

        /// <summary>
        /// 读连接串配置名称
        /// </summary>
        public string ReadingConnectionName { get; internal set; }

        /// <summary>
        /// 写连接串配置名称
        /// </summary>
        public string WritingConnectionName { get; internal set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DapperMetadata(Type entityType)
        {
            EntityType = entityType;
            Fields = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && CheckFieldType(p.PropertyType))
                .Select(p => new DapperFieldMetadata(p)).ToList();
            TableName = MappingStrategyParser.Parse(entityType.Name);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="typeConvention"></param>
        /// <param name="modelConvention"></param>
        public DapperMetadata(Type entityType, TypeConvention typeConvention, ModelConvention modelConvention)
        {
            Guard.ArgumentNotNull(entityType, nameof(entityType));
            EntityType = entityType;

            var metadatas = from prop in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            where prop.CanRead && prop.CanWrite && CheckFieldType(prop.PropertyType)
                            let convention = modelConvention.PropertyConventions.FirstOrDefault(x => x.Filter(prop))
                            select convention != null ? new DapperFieldMetadata(prop, convention) : new DapperFieldMetadata(prop);

            Fields = metadatas.ToList();
            TableName = MappingStrategyParser.Parse(entityType.Name);

            if (typeConvention.Filter(entityType))
            {
                ReadingConnectionName = typeConvention.DbReadingConnectionName;
                WritingConnectionName = typeConvention.DbWritingConnectionName;
            }
        }

        /// <summary>
        /// 校验字段类型是否符合规范
        /// </summary>
        /// <param name="type">字段类型</param>
        /// <returns></returns>
        private static bool CheckFieldType(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(byte[]) ||
                   type == typeof(decimal) ||
                   type == typeof(Guid) ||
                   type == typeof(DateTime) ||
                   type == typeof(TimeSpan) ||
                   type.IsEnum ||
                   Nullable.GetUnderlyingType(type) != null && CheckFieldType(Nullable.GetUnderlyingType(type));
        }
    }
}
