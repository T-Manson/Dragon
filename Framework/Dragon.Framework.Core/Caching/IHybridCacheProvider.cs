namespace Dragon.Framework.Core.Caching
{
    /// <summary>
    /// 混合缓存提供者
    /// </summary>
    public interface IHybridCacheProvider : ICacheManager
    {
        /// <summary>
        /// 缓存级别
        /// </summary>
        CacheLevel CacheLevel { get; }
    }
}
