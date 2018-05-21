using System;
using Dragon.Framework.Core.Caching;

namespace Dragon.Framework.Caching.Redis
{
    /// <summary>
    /// Redis管理接口
    /// </summary>
    public interface IRedisCacheManager : IHybridCacheProvider, IDisposable
    {
    }
}
