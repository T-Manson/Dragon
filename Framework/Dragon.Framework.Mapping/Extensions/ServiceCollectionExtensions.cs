using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Dragon.Framework.Mapping.Extensions
{
    /// <summary>
    /// ServiceCollection扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加AutoMapper注入支持
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="searchPattern">程序集匹配范围</param>
        /// <returns></returns>
        public static void AddAutoMapper(this IServiceCollection services, string searchPattern)
        {
            services.AddSingleton(c => Mapper.Instance);
            AutoMapperBootstrap.Initialize(searchPattern);
        }
    }
}
