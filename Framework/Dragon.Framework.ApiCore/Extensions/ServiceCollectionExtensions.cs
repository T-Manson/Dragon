using System;
using System.Linq;
using Dragon.Framework.Core.DependencyInjection;
using Dragon.Framework.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Dragon.Framework.ApiCore.Extensions
{
    /// <summary>
    /// ServiceCollection扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册所有需要注入的服务
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="searchPattern">程序集匹配范围</param>
        /// <returns></returns>
        public static IServiceCollection RegisterServiceAll(this IServiceCollection services, string searchPattern)
        {
            //Common
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //From Assembly
            var assemblies = AssemblyHelper.GetAssembliesBySearchPattern(searchPattern);

            var scopedBaseType = typeof(IDependency);
            foreach (var assembly in assemblies)
            {
                // Scoped
                var types = assembly.ExportedTypes.Where(type => !type.IsAbstract && scopedBaseType.IsAssignableFrom(type)).ToList();
                if (types.Any())
                {
                    foreach (var type in types)
                    {
                        var diInterfaceType = type.GetInterfaces().FirstOrDefault(interfaceType => scopedBaseType.IsAssignableFrom(interfaceType));
                        if (diInterfaceType != null) services.AddScoped(diInterfaceType, type);
                    }
                }
            }
            Console.WriteLine("DI注入完成。");

            return services;
        }
    }
}
