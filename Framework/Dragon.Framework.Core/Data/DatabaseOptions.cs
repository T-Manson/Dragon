using Dragon.Framework.Core.Exceptions;
using Dragon.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dragon.Framework.Core.Data
{
    /// <summary>
    /// 表示数据库选项。
    /// </summary>
    public class DatabaseOptions
    {
        /// <summary>
        /// 默认数据库连接配置名
        /// </summary>
        private string _defaultConnectionName;

        /// <summary>
        /// 连接
        /// </summary>
        private Dictionary<string, DatabaseConnectionOptions> _connectionStrings;

        /// <summary>
        /// 获取或设置默认数据库连接字符串的键名称。
        /// </summary>
        public virtual string DefaultConnectionName
        {
            get => _defaultConnectionName.IfNullOrWhiteSpace(ConnectionStrings.Keys.FirstOrDefault());
            set => _defaultConnectionName = value;
        }

        /// <summary>
        /// 获取或设置连接字符串集合。
        /// </summary>
        public virtual Dictionary<string, DatabaseConnectionOptions> ConnectionStrings
        {
            get => _connectionStrings ?? (_connectionStrings = new Dictionary<string, DatabaseConnectionOptions>(StringComparer.Ordinal));
            set => _connectionStrings = value;
        }

        /// <summary>
        /// 获取数据库连接字符串。
        /// </summary>
        /// <returns></returns>
        public virtual DatabaseConnectionOptions GetDatabaseConnectionOption(string connectionName = null, bool throwIfNotExisted = true)
        {
            connectionName = connectionName.IfNullOrWhiteSpace(_defaultConnectionName);

            DatabaseConnectionOptions databaseConnectionOption;
            if (!connectionName.IsNullOrWhiteSpace())
            {
                databaseConnectionOption = ConnectionStrings.GetOrDefault(connectionName);
                if (databaseConnectionOption.ConnectionString.IsNullOrWhiteSpace() && throwIfNotExisted)
                    throw new ConfigException($"在配置中找不到名为 '{connectionName}' 的数据库连接字符串。");
            }
            else
            {
                databaseConnectionOption = ConnectionStrings.FirstOrDefault().Value;
                if (databaseConnectionOption.ConnectionString.IsNullOrWhiteSpace() && throwIfNotExisted)
                    throw new ConfigException("没有配置任何可用的数据库连接字符串，或连接字符串为空。");
            }

            return databaseConnectionOption;
        }
    }
}
