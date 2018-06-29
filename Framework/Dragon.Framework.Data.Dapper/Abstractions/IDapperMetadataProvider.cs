using Dragon.Framework.Core.DependencyInjection;
using Dragon.Framework.Data.Dapper.Metadata;

namespace Dragon.Framework.Data.Dapper.Abstractions
{
    /// <summary>
    /// 实现 Dapper 元数据提供程序。
    /// </summary>
    public interface IDapperMetadataProvider : ISingletonDependency
    {
        /// <summary>
        /// 获取 Dapper 持久化存储元数据。
        /// </summary>
        /// <returns></returns>
        DapperMetadata GetMetadata();
    }
}
