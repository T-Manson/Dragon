using Microsoft.Extensions.Configuration;

namespace Dragon.Framework.Core.Config
{
    /// <inheritdoc />
    /// <summary>
    /// 配置中心数据源
    /// </summary>
    public class ConfigCenterConfigurationSource : IConfigurationSource
    {

        /// <inheritdoc />
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ConfigCenterConfigurationProvider();
        }
    }
}
