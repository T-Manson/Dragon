using System;
using System.Linq;
using AutoMapper;
using Dragon.Framework.Infrastructure.Helpers;

namespace Dragon.Framework.Mapping
{
    /// <summary>
    /// 映射帮助类
    /// </summary>
    public static class AutoMapperBootstrap
    {
        ///// <summary>
        ///// 注入AutoMapper对象 TODO
        ///// </summary>
        //public static void RegisterMapperService(ContainerBuilder containerBuilder)
        //{
        //    containerBuilder.Register(c => Mapper.Instance);
        //    Console.WriteLine("注入AutoMapper对象完成。");
        //}

        /// <summary>
        /// 初始化实体映射配置
        /// </summary>
        /// <param name="searchPattern">程序集匹配规则</param>
        public static void Initialize(string searchPattern)
        {
            var assemblies = AssemblyHelper.GetAssembliesBySearchPattern(searchPattern);
            var profileType = typeof(Profile);

            Mapper.Initialize(x =>
            {
                foreach (var assembly in assemblies)
                {
                    var types = assembly.ExportedTypes.Where(type => !type.IsAbstract && profileType.IsAssignableFrom(type)).ToList();
                    if (types.Any())
                    {
                        x.AddProfiles(types);
                    }
                }
            });

            Console.WriteLine("AutoMapper映射关系添加完成。");
        }
    }
}
