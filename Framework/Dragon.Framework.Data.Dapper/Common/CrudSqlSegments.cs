using Dragon.Framework.Data.Dapper.Metadata;
using Dragon.Framework.Infrastructure;
using System;
using System.Linq;
using System.Text;

namespace Dragon.Framework.Data.Dapper.Common
{
    /// <summary>
    /// 表示基本的增删改查 SQL 语句。
    /// </summary>
    public class CrudSqlSegments
    {
        private readonly Lazy<String> _insertSqlLazy;
        /// <summary>
        /// 插入的SQL
        /// </summary>
        public string InsertSql => _insertSqlLazy.Value;

        private readonly Lazy<String> _updateSqlLazy;
        /// <summary>
        /// 更新的SQL
        /// </summary>
        public string UpdateSql => _updateSqlLazy.Value;

        private readonly Lazy<String> _deleteSqlLazy;
        /// <summary>
        /// 删除的SQL
        /// </summary>
        public string DeleteSql => _deleteSqlLazy.Value;

        private readonly Lazy<String> _selectSqlLazy;
        /// <summary>
        /// 查询的SQL
        /// </summary>

        public string SelectSql => _selectSqlLazy.Value;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="runtime">Dapper运行时</param>
        public CrudSqlSegments(Type entityType, DapperRuntime runtime)
        {
            Guard.ArgumentNotNull(entityType, nameof(entityType));

            var metadata = runtime.GetMetadata(entityType);
            _insertSqlLazy = new Lazy<string>(() => GenerateInsertSql(entityType, runtime, metadata));
            _updateSqlLazy = new Lazy<string>(() => GenerateUpdateSql(entityType, runtime, metadata));
            _deleteSqlLazy = new Lazy<string>(() => GenerateDeleteSql(entityType, runtime, metadata));
            _selectSqlLazy = new Lazy<string>(() => GenerateSelectSql(entityType, runtime, metadata));
        }

        #region Static Generate Sql Method

        /// <summary>
        /// 使用界定符包围T-SQL中的参数字符串。
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="runtime">Dapper运行时</param>
        /// <param name="metadata">Dapper元数据</param>
        /// <returns></returns>
        private static string KeyDelimitSegment(Type entityType, DapperRuntime runtime, DapperMetadata metadata)
        {
            return metadata.Fields.Where(f => f.IsKey).Select(k => $"{runtime.DelimitIdentifier(entityType, k.Name)} = {runtime.DelimitParameter(entityType, k.Field.Name)}").ToArrayString(" AND ");
        }

        /// <summary>
        /// 生成插入的SQL语句
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="runtime">Dapper运行时</param>
        /// <param name="metadata">Dapper元数据</param>
        /// <returns></returns>
        private static string GenerateInsertSql(Type entityType, DapperRuntime runtime, DapperMetadata metadata)
        {
            var fields = metadata.Fields.Where(f => !f.Ignore && !f.AutoGeneration).OrderBy(x => x.Field.Name).ToArray();
            string columns = fields.Select(k => runtime.DelimitIdentifier(entityType, k.Name)).ToArrayString(", ");

            string parameters = fields.Select(k => $"{runtime.DelimitParameter(entityType, k.Field.Name)}").ToArrayString(", ");

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("INSERT INTO ");
            sqlBuilder.Append($"{runtime.DelimitIdentifier(entityType, metadata.TableName)} ");
            sqlBuilder.Append($"({columns}) ");
            sqlBuilder.Append(" VALUES ");
            sqlBuilder.Append($"({parameters})");

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 生成更新的SQL语句
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="runtime">Dapper运行时</param>
        /// <param name="metadata">Dapper元数据</param>
        /// <returns></returns>
        private static string GenerateUpdateSql(Type entityType, DapperRuntime runtime, DapperMetadata metadata)
        {
            var fields = metadata.Fields.Where(f => !f.IsKey && !f.AutoGeneration && !f.Ignore).OrderBy(x => x.Field.Name).ToArray();
            string columnsSetSeg = fields.Select(k =>
                $"{runtime.DelimitIdentifier(entityType, k.Name)} = {runtime.DelimitParameter(entityType, k.Field.Name)}").ToArrayString(", ");

            string whereSeg = KeyDelimitSegment(entityType, runtime, metadata);

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("UPDATE ");
            sqlBuilder.Append($"{runtime.DelimitIdentifier(entityType, metadata.TableName)} ");
            sqlBuilder.Append("SET ");
            sqlBuilder.Append(columnsSetSeg);
            if (!string.IsNullOrWhiteSpace(whereSeg))
            {
                sqlBuilder.Append(" WHERE ");
                sqlBuilder.Append(whereSeg);
            }
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 生成删除的SQL语句
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="runtime">Dapper运行时</param>
        /// <param name="metadata">Dapper元数据</param>
        /// <returns></returns>
        private static string GenerateDeleteSql(Type entityType, DapperRuntime runtime, DapperMetadata metadata)
        {
            string whereSeg = KeyDelimitSegment(entityType, runtime, metadata);

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("DELETE FROM ");
            sqlBuilder.Append($"{runtime.DelimitIdentifier(entityType, metadata.TableName)} ");
            if (!string.IsNullOrWhiteSpace(whereSeg))
            {
                sqlBuilder.Append(" WHERE ");
                sqlBuilder.Append(whereSeg);
            }
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 生成查询的SQL语句
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="runtime">Dapper运行时</param>
        /// <param name="metadata">Dapper元数据</param>
        /// <returns></returns>
        private static string GenerateSelectSql(Type entityType, DapperRuntime runtime, DapperMetadata metadata)
        {
            string columnsSetSeg =
            metadata.Fields.Where(f => !f.Ignore).OrderBy(x => x.Field.Name)
                .Select(k => runtime.DelimitIdentifier(entityType, k.Name)).ToArrayString(", ");

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT ");
            sqlBuilder.Append(columnsSetSeg);
            sqlBuilder.Append(" FROM ");
            sqlBuilder.Append($"{runtime.DelimitIdentifier(entityType, metadata.TableName)}");

            return sqlBuilder.ToString();
        }

        #endregion
    }
}
