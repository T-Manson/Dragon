using Dragon.Framework.Core.Caching;
using System;

namespace Dragon.Framework.Caching.Hybrid
{
    /// <summary>
    /// 混合缓存管理接口
    /// </summary>
    public interface IHybridCacheManager : ICacheManager, IDisposable
    {
    }
}
