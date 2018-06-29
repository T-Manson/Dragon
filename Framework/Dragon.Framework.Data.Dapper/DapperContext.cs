using Dragon.Framework.Core.Data;
using Dragon.Framework.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// 表示一个 Dapper 上下文。
    /// </summary>
    public class DapperContext : IDatabaseContext, IDisposable
    {
        /// <summary>
        /// 获取 Dapper 运行时环境。
        /// </summary>
        public DapperRuntime Runtime { get; }

        /// <summary>
        /// 连接集合
        /// </summary>
        private readonly ConcurrentDictionary<string, DbConnection> _connections;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly Lazy<ILogger> _loggerLazy;

        /// <summary>
        /// 日志工厂
        /// </summary> 
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// 是否已经被回收
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 析构函数
        /// </summary>
        ~DapperContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="runtime">Dapper运行时</param>
        /// <param name="loggerFactory">日志工厂</param>
        public DapperContext(DapperRuntime runtime, ILoggerFactory loggerFactory)
        {
            Runtime = runtime;
            _loggerFactory = loggerFactory;
            _loggerLazy = new Lazy<ILogger>(() => loggerFactory?.CreateLogger<DapperContext>() ?? NullLogger<DapperContext>.Instance);
            _connections = new ConcurrentDictionary<string, DbConnection>();
        }

        /// <summary>
        /// 根据配置名称获取数据库连接。
        /// </summary>
        /// <param name="connectionName">配置名称，为空表示获取默认连接。</param>
        /// <param name="ensureOpenned">是否同时确保数据连接已经被打开。</param>
        /// <returns></returns>
        public DbConnection GetConnection(string connectionName = null, bool ensureOpenned = true)
        {
            try
            {
                var databaseConnectionOption = Runtime.GetDatabaseConnectionOption(connectionName);
                var provider = Runtime.GetDatabaseProvider(databaseConnectionOption);
                string cacheKey = connectionName.IfNullOrWhiteSpace("default");
                TrackingDbConnection trackingConn = null;
                var dbConnecion = _connections.GetOrAdd(cacheKey, connName =>
                {
                    string connectionString = databaseConnectionOption.ConnectionString;
                    ThrowIfConnectionNotFound(connName, connectionString);
                    var innerConn = provider.CreateConnection(connectionString);
                    trackingConn = new TrackingDbConnection(innerConn, _loggerFactory);
                    return trackingConn;
                });
                if (trackingConn != null && !ReferenceEquals(dbConnecion, trackingConn))
                {
                    trackingConn.Close();
                    trackingConn.Dispose();
                }
                if (ensureOpenned && dbConnecion.State == ConnectionState.Closed)
                {
                    dbConnecion.Open();
                }
                return dbConnecion;
            }
            catch (Exception exception)
            {
                _loggerLazy.Value.LogError(new EventId(GetHashCode(), "DapperContext"), exception, $"connectionName:{connectionName},ensureOpenned:{ensureOpenned}");
                if (exception.InnerException == null)
                    throw;
                throw exception.InnerException;
            }
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="level">事务级别</param>
        /// <param name="dbConnectionName">连接串名称</param>
        /// <returns></returns>
        public IDatabaseTransaction BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted, string dbConnectionName = null)
        {
            var transaction = GetConnection(dbConnectionName).BeginTransaction(level);
            return new DbTransactionWrapper(transaction);
        }

        /// <summary>
        /// 如果连接没找到则抛出异常
        /// </summary>
        /// <param name="connectionName">连接串配置名</param>
        /// <param name="connectionString">连接串</param>
        private static void ThrowIfConnectionNotFound(string connectionName, string connectionString)
        {
            if (connectionString.IsNullOrWhiteSpace())
            {
                if (connectionName.IsNullOrWhiteSpace())
                    throw new ArgumentException("找不到默认数据库连接字符串配置。");
                throw new ArgumentException($"找不到名为 {connectionName} 的数据库连接字符串配置。");
            }
        }

        #region 回收

        /// <summary>
        /// 回收
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="disposing">是否主动回收</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                var conns = _connections.Values.ToArray();
                foreach (var c in conns)
                {
                    if (c.State == ConnectionState.Open)
                    {
                        c.Close();
                    }
                    c.Dispose();
                }

                _connections.Clear();
            }
            _disposed = true;
        }

        #endregion
    }
}
