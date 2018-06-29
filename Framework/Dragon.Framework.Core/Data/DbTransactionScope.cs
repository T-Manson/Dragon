using Dragon.Framework.Core.Environment;
using Dragon.Framework.Core.Environment.State;
using Dragon.Framework.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Threading;

namespace Dragon.Framework.Core.Data
{
    /// <summary>
    /// DB事务作用域
    /// </summary>
    public class DbTransactionScope : IDisposable
    {
        /// <summary>
        /// 事务对象
        /// </summary>
        public IDatabaseTransaction Transaction { get; private set; }

        /// <summary>
        /// 是否已经销毁
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 是否完成
        /// </summary>
        private bool _completed;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="connectionName">连接配置名</param>
        public DbTransactionScope(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted, string connectionName = null)
        {
            TransactionState state = GetTransactionState();
            int count = Interlocked.Increment(ref state.ChainCount);
            if (count == 1)
            {
                var context = WorkContext.Current.RequestServices.GetRequiredService<IDatabaseContext>();
                var dbTransaction = context.BeginTransaction(isolationLevel, connectionName);
                Transaction = dbTransaction;
            }
        }

        /// <summary>
        /// 完成
        /// </summary>
        public void Complete()
        {
            TransactionState state = GetTransactionState();
            int count = Interlocked.Increment(ref state.CommitCount);
            if (count == state.ChainCount)
                Transaction.Commit();
            _completed = true;
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                TransactionState state = GetTransactionState();
                if (!_completed)
                {
                    Interlocked.Increment(ref state.RollbackCount);
                }
                if (state.RollbackCount + state.CommitCount == state.ChainCount)
                {
                    if (state.RollbackCount > 0)
                    {
                        Transaction?.Rollback();
                    }
                    state.ChainCount = 0;
                    state.CommitCount = 0;
                    state.RollbackCount = 0;
                }
                Transaction?.Dispose();
                Transaction = null;
            }
            _disposed = true;
        }

        /// <summary>
        /// 获取事务状态对象
        /// </summary>
        /// <returns></returns>
        private static TransactionState GetTransactionState()
        {
            var transactionState = WorkContext.GetState<TransactionState>(WorkContextStateName.TransactionState.GetDescription());
            Guard.ArgumentNotNull(transactionState, nameof(transactionState));

            return transactionState;
        }
    }
}
