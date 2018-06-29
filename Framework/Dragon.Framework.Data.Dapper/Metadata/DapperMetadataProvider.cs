using Dragon.Framework.Data.Dapper.Abstractions;

namespace Dragon.Framework.Data.Dapper.Metadata
{
    /// <summary>
    /// 派生类中实现实体到持久化存储的映射。
    /// </summary>
    public abstract class DapperMetadataProvider<T> : IDapperMetadataProvider
        where T : class
    {
        /// <summary>
        /// 应用映射。
        /// </summary>
        public DapperMetadata GetMetadata()
        {
            DapperMetadataBuilder<T> builder = new DapperMetadataBuilder<T>();
            ConfigureModel(builder);
            return builder.Build();
        }

        /// <summary>
        /// 派生类重写此方法用以配置实体到持久化存储的映射。
        /// </summary>
        /// <param name="builder"></param>
        protected abstract void ConfigureModel(DapperMetadataBuilder<T> builder);
    }
}
