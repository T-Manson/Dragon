using Dragon.Framework.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// 可跟踪的命令对象
    /// </summary>
    internal class TrackingDbCommand : DbCommand
    {
        /// <summary>
        /// 实际的命令对象
        /// </summary>
        private DbCommand _innerCommand;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly Lazy<ILogger> _loggerLazy;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="command">实际的命令对象</param>
        /// <param name="loggerFactory">日志工厂</param>
        public TrackingDbCommand(DbCommand command, ILoggerFactory loggerFactory)
            : this(command, loggerFactory?.CreateLogger("Database Tracking"))
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="command">实际的命令对象</param>
        /// <param name="logger">日志</param>
        public TrackingDbCommand(DbCommand command, ILogger logger)
        {
            Guard.ArgumentNotNull(command, nameof(command));

            _innerCommand = command;
            _loggerLazy = new Lazy<ILogger>(() => logger ?? NullLogger.Instance);
        }

        /// <summary>
        /// 命令文本
        /// </summary>
        public override string CommandText
        {
            get => _innerCommand.CommandText;
            set => _innerCommand.CommandText = value;
        }

        /// <summary>
        /// 命令执行超时时长
        /// </summary>
        public override int CommandTimeout
        {
            get => _innerCommand.CommandTimeout;
            set => _innerCommand.CommandTimeout = value;
        }

        /// <summary>
        /// 命令类型
        /// </summary>
        public override CommandType CommandType
        {
            get => _innerCommand.CommandType;
            set => _innerCommand.CommandType = value;
        }

        /// <summary>
        /// DesignTimeVisible
        /// </summary>
        public override bool DesignTimeVisible
        {
            get => _innerCommand.DesignTimeVisible;
            set => _innerCommand.DesignTimeVisible = value;
        }

        /// <summary>
        /// UpdatedRowSource
        /// </summary>
        public override UpdateRowSource UpdatedRowSource
        {
            get => _innerCommand.UpdatedRowSource;
            set => _innerCommand.UpdatedRowSource = value;
        }

        /// <summary>
        /// 连接对象
        /// </summary>
        protected override DbConnection DbConnection
        {
            get => _innerCommand.Connection;
            set => _innerCommand.Connection = value;
        }

        /// <summary>
        /// 事务对象
        /// </summary>
        protected override DbTransaction DbTransaction
        {
            get => _innerCommand.Transaction;
            set => _innerCommand.Transaction = value;
        }

        /// <summary>
        /// 命令参数集合
        /// </summary>
        protected override DbParameterCollection DbParameterCollection => _innerCommand.Parameters;

        /// <summary>
        /// 取消
        /// </summary>
        public override void Cancel()
        {
            _innerCommand.Cancel();
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public override int ExecuteNonQuery()
        {
            WriteFormattedLog();
            return _innerCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 查询单个单元格
        /// </summary>
        /// <returns></returns>
        public override object ExecuteScalar()
        {
            WriteFormattedLog();
            return _innerCommand.ExecuteScalar();
        }

        /// <summary>
        /// 异步执行
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            WriteFormattedLog();
            return _innerCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// 异步查询单个单元格
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            WriteFormattedLog();
            return _innerCommand.ExecuteScalarAsync(cancellationToken);
        }

        public override void Prepare()
        {
            _innerCommand.Prepare();
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
                _innerCommand?.Dispose();
                _innerCommand = null;
            }
        }

        /// <summary>
        /// 查询多行
        /// </summary>
        /// <param name="behavior">命令行为</param>
        /// <returns></returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            WriteFormattedLog();
            return _innerCommand.ExecuteReader(behavior);
        }

        /// <summary>
        /// 异步查询多行
        /// </summary>
        /// <param name="behavior">命令行为</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns></returns>
        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            WriteFormattedLog();
            return _innerCommand.ExecuteReaderAsync(behavior, cancellationToken);
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <returns></returns>
        protected override DbParameter CreateDbParameter()
        {
            return _innerCommand.CreateParameter();
        }

        /// <summary>
        /// 记录执行日志
        /// </summary>
        private void WriteFormattedLog()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("tracking T-SQL excuting");
            builder.AppendLine(CommandText);
            builder.AppendLine();
            builder.AppendLine("parameters:");
            if (Parameters.Count > 0)
            {
                builder.AppendLine(Parameters.Cast<IDbDataParameter>()
                    .Select(p => $"@{p.ParameterName} = {GetValueString(p)}").ToArrayString(", "));
            }
            string message = builder.ToString();
            _loggerLazy.Value.LogDebug(message);
        }

        /// <summary>
        /// 获取值对应的字符串
        /// </summary>
        /// <param name="parameter">参数对象</param>
        /// <returns></returns>
        private static string GetValueString(IDbDataParameter parameter)
        {
            return parameter.Value is DBNull || parameter.Value == null ? "NULL" : parameter.Value.ToString();
        }
    }
}
