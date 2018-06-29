using System;

namespace Dragon.Framework.Core.Data
{
    /// <summary>
    /// 数据库事务接口
    /// </summary>
    public interface IDatabaseTransaction : IDisposable
    {
        /// <summary>
        /// 提交
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚
        /// </summary>
        void Rollback();
    }
}
