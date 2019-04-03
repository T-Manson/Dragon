using Microsoft.Extensions.Configuration;

namespace Dragon.Framework.Core.Config
{
    /// <inheritdoc />
    /// <summary>
    /// 配置中心配置提供器 TODO ZooKeeper
    /// </summary>
    public class ConfigCenterConfigurationProvider : ConfigurationProvider
    {
        /// <inheritdoc />
        /// <summary>
        /// 加载配置
        /// </summary>
        public override void Load()
        {
            // TODO 从配置中心拉取配置至IConfiguration中
            Data = null;
        }
    }
}
