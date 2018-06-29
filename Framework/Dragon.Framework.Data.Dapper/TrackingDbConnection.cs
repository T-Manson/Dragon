using Dragon.Framework.Core;
using Dragon.Framework.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// 可跟踪的连接对象
    /// </summary>
    internal class TrackingDbConnection : DbConnection
    {
        /// <summary>
        /// 实际使用的连接对象
        /// </summary>
        private DbConnection _innerConnection;

        /// <summary>
        /// 日志工厂
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// 资源滥用检测
        /// </summary>
        private static MisuseDetector _misuseDetector;

        /// <summary>
        /// 单例锁对象
        /// </summary>
        private static readonly Object Locker = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="innerConnection">实际使用的连接对象</param>
        /// <param name="loggerFactory">日志工厂</param>
        public TrackingDbConnection(DbConnection innerConnection, ILoggerFactory loggerFactory)
        {
            Guard.ArgumentNotNull(innerConnection, nameof(innerConnection));

            _innerConnection = innerConnection;
            _loggerFactory = loggerFactory;
            CreateMisuseDetectorIfNotExisted(loggerFactory);
            _misuseDetector.Increase();
        }

        /// <summary>
        /// 连接串
        /// </summary>
        public override string ConnectionString
        {
            get => _innerConnection.ConnectionString;
            set => _innerConnection.ConnectionString = value;
        }

        /// <summary>
        /// DataSource
        /// </summary>
        public override string DataSource => _innerConnection.DataSource;

        /// <summary>
        /// Database
        /// </summary>
        public override string Database => _innerConnection.Database;

        /// <summary>
        /// ServerVersion
        /// </summary>
        public override string ServerVersion => _innerConnection.ServerVersion;

        /// <summary>
        /// 连接状态
        /// </summary>
        public override ConnectionState State => _innerConnection.State;

        /// <summary>
        /// 切换DB
        /// </summary>
        /// <param name="databaseName"></param>
        public override void ChangeDatabase(string databaseName)
        {
            _innerConnection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public override void Open()
        {
            if (_innerConnection.State == ConnectionState.Closed)
                _innerConnection.Open();
        }

        /// <summary>
        /// 异步打开连接
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns></returns>
        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            if (_innerConnection.State == ConnectionState.Closed)
                return _innerConnection.OpenAsync(cancellationToken);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public override void Close()
        {
            if (_innerConnection.State == ConnectionState.Open)
                _innerConnection.Close();
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns></returns>
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return _innerConnection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// 创建命令对象
        /// </summary>
        /// <returns></returns>
        protected override DbCommand CreateDbCommand()
        {
            var dbCommand = _innerConnection.CreateCommand();
            return new TrackingDbCommand(dbCommand, _loggerFactory);
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="disposing">是否主动回收</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _innerConnection?.Dispose();
                _innerConnection = null;
                _misuseDetector.Decrease();
            }
        }

        /// <summary>
        /// 如果资源滥用检测不存在则新建一个
        /// </summary>
        /// <param name="loggerFactory"></param>
        private static void CreateMisuseDetectorIfNotExisted(ILoggerFactory loggerFactory)
        {
            if (_misuseDetector == null)
            {
                lock (Locker)
                {
                    if (_misuseDetector == null)
                    {
                        _misuseDetector = new MisuseDetector(typeof(DbConnection), loggerFactory, 100);
                    }
                }
            }
        }
    }
}
