using System;
using System.Linq;
using System.Text;
using Dapper;
using Dragon.Framework.Infrastructure;

namespace Dragon.Framework.Data.Dapper.Models
{
    /// <summary>
    /// 执行的Sql对象
    /// </summary>
    internal class DapperExecuteSql
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DapperExecuteSql()
        {
            Parameters = new DynamicParameters();
        }

        /// <summary>
        /// 需要执行的Sql
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// Dapper参数
        /// </summary>
        public DynamicParameters Parameters { get; }

        /// <summary>
        /// 获取执行的Sql
        /// </summary>
        /// <returns></returns>
        public string GetSqlString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(Sql);
            builder.AppendLine("-------------------------------------------");
            builder.AppendLine(Parameters.ParameterNames.Select(n => $"@{n} = {Parameters.Get<Object>(n)?.ToString().IfNullOrWhiteSpace("null")}").ToArrayString(", "));
            return builder.ToString();
        }
    }
}
