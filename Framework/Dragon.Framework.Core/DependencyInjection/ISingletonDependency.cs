namespace Dragon.Framework.Core.DependencyInjection
{
    /// <summary>
    /// 标记一个类型为依赖项（使用此接口的依赖项的生命周期为 singleton）。
    /// </summary>
    public interface ISingletonDependency : IDependency
    {
    }
}
