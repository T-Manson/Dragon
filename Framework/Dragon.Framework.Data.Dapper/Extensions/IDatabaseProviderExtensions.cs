using Dragon.Framework.Data.Dapper.Abstractions;

namespace Dragon.Framework.Data.Dapper.Extensions
{
    /// <summary>
    /// 数据提供类扩展
    /// </summary>
    public static class DatabaseProviderExtensions
    {
        /// <summary>
        /// 使用界定符包围T-SQL中的主体字符串。
        /// </summary>
        /// <param name="provider">数据提供类</param>
        /// <param name="dbIdentifier">db查询库、字段对应名称</param>
        /// <returns></returns>
        public static string DelimitIdentifier(this IDatabaseProvider provider, string dbIdentifier) =>
            $"{provider.IdentifierPrefix}{dbIdentifier}{provider.IdentifierStuffix}";

        /// <summary>
        /// 使用界定符包围T-SQL中的参数字符串。
        /// </summary>
        /// <param name="provider">数据提供类</param>
        /// <param name="parameterName">参数名</param>
        /// <returns></returns>
        public static string DelimitParameter(this IDatabaseProvider provider, string parameterName) =>
            $"{provider.ParameterPrefix}{parameterName}";
    }
}
