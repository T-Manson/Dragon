using Microsoft.Extensions.Configuration;
using Dragon.Framework.Core.Exceptions;
using Dragon.Framework.Infrastructure;
using System;

namespace Dragon.Framework.Core.Config
{
    /// <summary>
    /// 配置中心扩展
    /// </summary>
    public static class ConfigExtensions
    {
        /// <summary>
        /// 添加通用配置
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddNormalConfig(this IConfigurationBuilder builder)
        {
            var environment = System.Environment.GetEnvironmentVariable("SYSTEM_ENVIRONMENT");
            builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment}.json", true, true);
            return builder;
        }

        /// <summary>
        /// 添加通用配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="basePath">根目录</param>
        /// <param name="environment">环境值</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddNormalConfig(this IConfigurationBuilder builder, 
            string basePath, string environment)
        {
            builder.SetBasePath(basePath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment}.json", true, true);
            return builder;
        }
    }
}
