namespace Dragon.Framework.Core.Environment.State
{
    /// <summary>
    /// 工作上下文状态接口
    /// </summary>
    public interface IWorkContextState
    {
        /// <summary>
        /// 状态名称
        /// </summary>
        string StateName { get; }
    }
}
