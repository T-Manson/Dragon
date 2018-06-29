using Dragon.Framework.Infrastructure;
using System.Data.Common;

namespace Dragon.Framework.Core.Data
{
    /// <summary>
    /// 事务对象包装类
    /// </summary>
    public class DbTransactionWrapper : IDatabaseTransaction
    {
        /// <summary>
        /// 实际的事务对象
        /// </summary>
        private DbTransaction _innerTransaction;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbTransaction">实际的事务对象</param>
        public DbTransactionWrapper(DbTransaction dbTransaction)
        {
            Guard.ArgumentNotNull(dbTransaction, nameof(dbTransaction));

            _innerTransaction = dbTransaction;
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            _innerTransaction?.Commit();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void Rollback()
        {
            _innerTransaction?.Rollback();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Dispose()
        {
            _innerTransaction?.Dispose();
            _innerTransaction = null;
        }
    }
}
