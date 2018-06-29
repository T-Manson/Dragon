namespace Dragon.Framework.Core.Data
{
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    public class DatabaseConnectionOptions
    {
        /// <summary>
        /// 连接串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 数据提供者类型
        /// </summary>
        public DatabaseProviderType DatabaseProviderType { get; set; } = DatabaseProviderType.MySql;
    }
}
