using Dragon.Framework.Core.Environment.State;
using Dragon.Framework.Infrastructure;

namespace Dragon.Framework.Core.Data
{
    /// <summary>
    /// 事务状态
    /// </summary>
    internal sealed class TransactionState : IWorkContextState
    {
        /// <summary>
        /// 状态名称
        /// </summary>
        public string StateName => WorkContextStateName.TransactionState.GetDescription();

        /// <summary>
        /// 事务块数量
        /// </summary>
        public int ChainCount;

        /// <summary>
        /// 提交数量
        /// </summary>
        public int CommitCount;

        /// <summary>
        /// 回滚数量
        /// </summary>
        public int RollbackCount;
    }
}
