namespace Dragon.Framework.Core.DependencyInjection
{
    /// <summary>
    /// 标记一个类型为依赖项（使用此接口的依赖项的生命周期为 per usage）。
    /// </summary>
    public interface ITransientDependency : IDependency
    {
    }
}
