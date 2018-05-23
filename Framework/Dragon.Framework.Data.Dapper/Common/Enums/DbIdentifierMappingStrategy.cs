namespace Dragon.Framework.Data.Dapper.Common.Enums
{
    /// <summary>
    /// DB命名映射策略
    /// </summary>
    public enum DbIdentifierMappingStrategy
    {
        /// <summary>
        /// 驼峰命名规则
        /// </summary>
        PascalCase = 0,

        /// <summary>
        /// 下划线分隔
        /// </summary>
        Underline = 1
    }
}
