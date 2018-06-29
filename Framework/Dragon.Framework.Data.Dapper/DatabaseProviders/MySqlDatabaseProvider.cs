using Dragon.Framework.Data.Dapper.Abstractions;
using Dragon.Framework.Data.Dapper.Extensions;
using Dragon.Framework.Infrastructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Dragon.Framework.Data.Dapper.DatabaseProviders
{
    /// <summary>
    /// MySql 数据提供者
    /// </summary>
    public sealed class MySqlDatabaseProvider : IDatabaseProvider
    {
        /// <summary>
        /// <see cref="IDatabaseProvider.GetLastedInsertIdSupported"/>
        /// </summary>
        public bool GetLastedInsertIdSupported => true;

        /// <summary>
        /// <see cref="IDatabaseProvider.IdentifierPrefix"/>
        /// </summary>
        public string IdentifierPrefix => "`";

        /// <summary>
        /// <see cref="IDatabaseProvider.IdentifierStuffix"/>
        /// </summary>
        public string IdentifierStuffix => "`";

        /// <summary>
        /// <see cref="IDatabaseProvider.ParameterPrefix"/>
        /// </summary>
        public string ParameterPrefix => "@";

        /// <summary>
        /// <see cref="IDatabaseProvider.BuildBatchInsertSqlSupported"/>
        /// </summary>
        public bool BuildBatchInsertSqlSupported => true;

        /// <summary>
        /// <see cref="IDatabaseProvider.CreateConnection"/>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// <see cref="IDatabaseProvider.BuildBatchInsertSql"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <param name="buildDbParameter"></param>
        /// <returns></returns>
        public string BuildBatchInsertSql(string tableName, string[] columns, IEnumerable<object[]> values, Action<String, object> buildDbParameter)
        {
            var valueList = values.ToArray();
            if (valueList.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(values), "BuildBatchInsertSql 参数 values 不能为空或空数组。");
            if (columns.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(columns), "BuildBatchInsertSql 参数 columns 不能为空或空数组。");

            int columnCount = columns.Length;
            if (valueList.Any(v => v.Length != columnCount))
                throw new ArgumentException("BuildBatchInsertSql 参数 columns 的长度和 values 中数组的长度不一致，即参数值的数量和列的数量不一致。");

            string columnNames = columns.Select(this.DelimitIdentifier).ToArrayString(", ");

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("INSERT INTO ");
            sqlBuilder.Append($"{this.DelimitIdentifier(tableName)} ");
            sqlBuilder.Append($"({columnNames})");
            sqlBuilder.Append(" VALUES ");

            int valueIndex = 0;
            foreach (var value in valueList)
            {
                if (valueIndex > 0)
                {
                    sqlBuilder.Append(", ");
                }

                var parameters = new string[columnCount];
                for (var i = 0; i < columnCount; ++i)
                {
                    parameters[i] = this.DelimitParameter($"{columns[i]}{valueIndex}");
                    buildDbParameter?.Invoke(parameters[i], value[i]);
                }
                sqlBuilder.Append($"({parameters.ToArrayString(", ")})");
                valueIndex++;
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// <see cref="IDatabaseProvider.BuildPaginationTSql"/>
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectSegment"></param>
        /// <param name="orderBySegment"></param>
        /// <param name="whereSegment"></param>
        /// <returns></returns>
        public string BuildPaginationTSql(int pageIndex, int pageSize, string selectSegment, string orderBySegment, string whereSegment = null)
        {
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(pageIndex)} 参数必须大于等于 0。");

            Guard.ArgumentNullOrWhiteSpaceString(selectSegment, nameof(selectSegment));

            string primarySql = selectSegment;
            if (!whereSegment.IsNullOrWhiteSpace())
            {
                primarySql = string.Concat(primarySql, $" WHERE {whereSegment}");
            }
            if (!orderBySegment.IsNullOrWhiteSpace())
            {
                primarySql = string.Concat(primarySql, $" {orderBySegment}");
            }

            if (pageIndex == 0)
            {
                return $"{primarySql} LIMIT {pageSize}";
            }

            int skip = pageIndex * pageSize;
            return $"{primarySql} LIMIT {skip}, {pageSize}";
        }

        /// <summary>
        /// <see cref="IDatabaseProvider.GetLastedInsertId"/>
        /// </summary>
        /// <returns></returns>
        public object GetLastedInsertId()
        {
            return "SELECT LAST_INSERT_ID()";
        }
    }
}
