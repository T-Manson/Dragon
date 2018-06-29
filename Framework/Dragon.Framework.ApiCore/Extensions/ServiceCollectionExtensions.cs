using Dragon.Framework.Core.DependencyInjection;
using Dragon.Framework.Infrastructure;
using Dragon.Framework.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

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

            var singletonBaseType = typeof(ISingletonDependency);
            var transientBaseType = typeof(ITransientDependency);
            foreach (var assembly in assemblies)
            {
                foreach (var exportedType in assembly.ExportedTypes)
                {
                    if (exportedType.IsAbstract) continue;

                    var dependencyInterfaceTypeList = DependencyInjectionHelper.GetDependencyInterfaceTypeList(exportedType.GetInterfaces())?.ToList();
                    if (dependencyInterfaceTypeList.IsNullOrEmpty()) continue;

                    foreach (var diInterfaceType in dependencyInterfaceTypeList)
                    {
                        if (diInterfaceType != null)
                        {
                            if (singletonBaseType.IsAssignableFrom(exportedType))
                            {
                                services.AddSingleton(diInterfaceType, exportedType);
                            }
                            else if (transientBaseType.IsAssignableFrom(exportedType))
                            {
                                services.AddTransient(diInterfaceType, exportedType);
                            }
                            else
                            {
                                services.AddScoped(diInterfaceType, exportedType);
                            }

                            PrintConsole(diInterfaceType, exportedType);
                        }
                    }
                }
            }

            Console.WriteLine("DI注入完成。");

            return services;
        }

        /// <summary>
        /// 打印控制台
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="implementType">实现的接口类型</param>
        private static void PrintConsole(Type interfaceType, Type implementType)
        {
            Console.WriteLine($"{interfaceType} <- {implementType} OK.");
        }
    }
}
