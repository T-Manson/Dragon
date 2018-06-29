using Dragon.Framework.Data.Dapper.Common;
using Dragon.Framework.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dragon.Framework.Data.Dapper.Metadata
{
    /// <summary>
    /// 实体映射构造器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DapperMetadataBuilder<T>
        where T : class
    {
        /// <summary>
        /// Dapper元数据
        /// </summary>
        private readonly DapperMetadata _dapperMetadata;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DapperMetadataBuilder()
        {
            _dapperMetadata = new DapperMetadata(typeof(T));
        }

        /// <summary>
        /// 指定表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public DapperMetadataBuilder<T> SetTableName(string tableName)
        {
            Guard.ArgumentNullOrWhiteSpaceString(tableName, nameof(tableName));

            _dapperMetadata.TableName = tableName;
            return this;
        }

        /// <summary>
        /// 指定实体的某个属性值是通过DB自动生成。
        /// </summary>
        /// <param name="expression">表示实体属性的表达式（示例：a=>a.P ）。</param>
        /// <returns></returns>
        public DapperMetadataBuilder<T> SetAutoGeneration(Expression<Func<T, object>> expression)
        {
            Guard.ArgumentNotNull(expression, nameof(expression));

            Expression visited = expression.Body;
            if (visited is UnaryExpression unary) visited = unary.Operand;

            if (visited is MemberExpression mex)
            {
                string name = MappingStrategyParser.Parse(mex.Member.Name);
                var foundField = _dapperMetadata.Fields.FirstOrDefault(k => k.Name.CaseInsensitiveEquals(name));
                if (foundField != null) foundField.AutoGeneration = true;
            }
            else
                throw new ArgumentException($@"{nameof(DapperMetadataBuilder<T>)}.{nameof(SetAutoGeneration)} 方法只接受单个属性表达式，例如 a=>a.Property。");

            return this;
        }

        /// <summary>
        /// 指定实体的主键。
        /// </summary>
        /// <param name="expression">表示实体键的表达式（示例：a=>a.P ，当需要复合键时使用 a=>new { a.P1, a.P2 }）。</param>
        /// <returns></returns> 
        public DapperMetadataBuilder<T> SetPrimaryKey(Expression<Func<T, object>> expression)
        {
            Guard.ArgumentNotNull(expression, nameof(expression));

            foreach (var f in _dapperMetadata.Fields)
                f.IsKey = false;

            Expression visited = expression.Body;
            if (visited is UnaryExpression unary) visited = unary.Operand;

            if (visited is MemberExpression mex)
            {
                string name = MappingStrategyParser.Parse(mex.Member.Name);
                var keyField = _dapperMetadata.Fields.FirstOrDefault(p => p.Name.CaseInsensitiveEquals(name));
                if (keyField == null)
                    throw new ArgumentException($"{nameof(DapperMetadataBuilder<T>)}.{nameof(SetPrimaryKey)} 方法传入的属性表达式找不到属性 {name}。");

                if (keyField.Ignore)
                    throw new ArgumentException($"{nameof(DapperMetadataBuilder<T>)}.{nameof(SetPrimaryKey)} 不能指定一个已忽略的属性作为 Key 。");

                keyField.IsKey = true;
                return this;
            }

            if (visited is NewExpression newExpression)
            {
                var propertyNames = newExpression.Members.Select(m => MappingStrategyParser.Parse(m.Name)).ToArray();
                var keyProperties = _dapperMetadata.Fields.Where(f => propertyNames.Any(p => p.CaseInsensitiveEquals(f.Name))).ToArray();
                if (!keyProperties.Any())
                    throw new ArgumentException($"{nameof(DapperMetadataBuilder<T>)}.{nameof(SetPrimaryKey)} 初始化对象表达式中至少要指定一个属性。");

                if (keyProperties.Any(k => k.Ignore))
                    throw new ArgumentException($"{nameof(DapperMetadataBuilder<T>)}.{nameof(SetPrimaryKey)} 不能指定已忽略的属性作为 Key 。");

                keyProperties.ForEach(f => f.IsKey = true);
                return this;
            }

            throw new ArgumentException($"{nameof(SetPrimaryKey)} 方法使用了不支持的表达式作为 {nameof(expression)} 参数。");
        }

        /// <summary>
        /// 指定实体使用特定的数据源（数据库）。
        /// </summary>
        /// <param name="connectionName">数据库的连接字符串名称（配置中的连接名称）。</param>
        /// <returns></returns>
        public DapperMetadataBuilder<T> SetDataSource(string connectionName)
        {
            return SetDataSource(connectionName, connectionName);
        }

        /// <summary>
        /// 指定实体使用特定的数据源（数据库）。
        /// </summary>
        /// <param name="readingConnectionName">读库的连接字符串名称（配置中的连接名称）。</param>
        /// <param name="writingConnectionName">写库的连接字符串名称（配置中的连接名称）。</param>
        /// <returns></returns>
        public DapperMetadataBuilder<T> SetDataSource(string readingConnectionName, string writingConnectionName)
        {
            _dapperMetadata.ReadingConnectionName = readingConnectionName;
            _dapperMetadata.WritingConnectionName = writingConnectionName;
            return this;
        }

        /// <summary>
        /// 忽略实体属性。
        /// </summary>
        /// <param name="propertyExpression">表示实体属性的表达式（示例：a=>a.P ）。</param>
        /// <returns></returns>
        public DapperMetadataBuilder<T> SetIgnore(Expression<Func<T, object>> propertyExpression)
        {
            Guard.ArgumentNotNull(propertyExpression, nameof(propertyExpression));

            Expression visited = propertyExpression.Body;
            if (visited is UnaryExpression unary) visited = unary.Operand;

            if (visited is MemberExpression mex)
            {
                string name = mex.Member.Name;
                var field = _dapperMetadata.Fields.FirstOrDefault(k => k.Name.CaseInsensitiveEquals(name));
                if (field != null)
                {
                    if (field.IsKey)
                    {
                        throw new ArgumentException($"不能使用 {nameof(DapperMetadataBuilder<T>)}.{nameof(SetIgnore)} 方法忽略已经被指定为 Key 的属性。");
                    }
                    field.Ignore = true;
                }
            }
            else
                throw new ArgumentException($@"{nameof(DapperMetadataBuilder<T>)}.{nameof(SetIgnore)} 方法只接受单个属性表达式，例如 a=>a.Property。");

            return this;
        }

        /// <summary>
        /// 获取对应实体的元数据
        /// </summary>
        /// <returns></returns>
        internal DapperMetadata Build()
        {
            if (!_dapperMetadata.Fields.Any(f => f.IsKey))
                throw new InvalidOperationException($"没有为 Dapper 实体类型 {typeof(T).Name} 指定主键（请使用 {nameof(DapperMetadataBuilder<T>)}.{nameof(SetPrimaryKey)} 方法指定）。");

            return _dapperMetadata;
        }
    }
}
