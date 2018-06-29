using Dragon.Framework.Data.Dapper.Abstractions;
using Dragon.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace Dragon.Framework.Data.Dapper.DatabaseProviders
{
    /// <summary>
    /// Sql Server 数据提供者
    /// </summary>
    public sealed class SqlServerDatabaseProvider : IDatabaseProvider
    {
        /// <summary>
        /// <see cref="IDatabaseProvider.GetLastedInsertIdSupported"/>
        /// </summary>
        public bool GetLastedInsertIdSupported => true;

        /// <summary>
        /// <see cref="IDatabaseProvider.IdentifierPrefix"/>
        /// </summary>
        public string IdentifierPrefix => "[";

        /// <summary>
        /// <see cref="IDatabaseProvider.IdentifierStuffix"/>
        /// </summary>
        public string IdentifierStuffix => "]";

        /// <summary>
        /// <see cref="IDatabaseProvider.ParameterPrefix"/>
        /// </summary>
        public string ParameterPrefix => "@";

        /// <summary>
        /// <see cref="IDatabaseProvider.BuildBatchInsertSqlSupported"/>
        /// </summary>
        public bool BuildBatchInsertSqlSupported => false;

        /// <summary>
        /// <see cref="IDatabaseProvider.CreateConnection"/>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// <see cref="IDatabaseProvider.BuildBatchInsertSql"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <param name="buildDbParameter"></param>
        /// <returns></returns>
        public string BuildBatchInsertSql(string tableName, string[] columns, IEnumerable<object[]> values, Action<string, object> buildDbParameter)
        {
            return string.Empty;
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
            {
                throw new ArgumentOutOfRangeException($"{nameof(pageIndex)} 参数必须大于等于 0。");
            }

            Guard.ArgumentNullOrWhiteSpaceString(selectSegment, nameof(selectSegment));
            Guard.ArgumentNullOrWhiteSpaceString(orderBySegment, nameof(orderBySegment));

            string spliter = " FROM ";
            int index = selectSegment.IndexOf(spliter, StringComparison.OrdinalIgnoreCase);
            if (index > 0)
            {
                var selectSplited = selectSegment.Split(new[] { spliter }, StringSplitOptions.None);
                if (selectSplited.Length == 2)
                {
                    var clause = $"{selectSplited[0].Trim()}, ROW_NUMBER() OVER({orderBySegment}) AS [ROWNUMBER] FROM {selectSplited[1].Trim()}";
                    if (!whereSegment.IsNullOrWhiteSpace())
                    {
                        clause = string.Concat(clause, $" WHERE {whereSegment}");
                    }
                    clause = string.Concat(clause, $" {orderBySegment}");
                    string sql = $"{selectSplited[0].Trim()} FROM ({clause}) WHERE [ROWNUMBER] BETWEEN {pageIndex * pageSize + 1} AND {(pageIndex + 1) * pageSize}";
                    return sql;
                }
            }
            throw new ArgumentException($"{selectSegment} 不是有效的 TSQL 查询语句，请确保 FROM 前后包含空格。");

            /* example
             * select * from ( 
　　　　           select *, ROW_NUMBER() OVER(Order by a.CreateTime DESC ) AS RowNumber from table_name as a 
　　           ) as b 
　　           where RowNumber BETWEEN 1 and 5 
             */
        }

        /// <summary>
        /// <see cref="IDatabaseProvider.GetLastedInsertId"/>
        /// </summary>
        /// <returns></returns>
        public object GetLastedInsertId()
        {
            return "SELECT SCOPE_IDENTITY()";
        }
    }
}
