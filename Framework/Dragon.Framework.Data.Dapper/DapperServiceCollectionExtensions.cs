using Dragon.Framework.Core.Data;
using Dragon.Framework.Core.Exceptions;
using Dragon.Framework.Data.Dapper.Abstractions;
using Dragon.Framework.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dragon.Framework.Data.Dapper
{

    public static class DapperServiceCollectionExtensions
    {
        /// <summary>
        /// 添加以 Dapper 作为持久化的数据层特性。
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddDapper(this IServiceCollection services, IConfiguration configuration)
        {
            //修改dapper的默认映射规则,让其支持下划线列名到C#实体驼峰命名属性
            global::Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            var dataConfiguration = configuration.GetSection("Data");

            DapperDatabaseOptions dbOptions = new DapperDatabaseOptions();
            dataConfiguration.Bind(dbOptions);
            if (dbOptions.ConnectionStrings.IsNullOrEmpty())
                throw new ConfigException("Data:ConnectionStrings");

            services.Configure<DapperDatabaseOptions>(dataConfiguration);

            services.AddScoped<IDatabaseContext, DapperContext>();
            services.AddScoped(typeof(IRepository<>), typeof(DapperRepository<>));
            services.AddScoped<DapperContext, DapperContext>();
            services.AddSingleton<DapperRuntime, DapperRuntime>();

            Console.WriteLine("Dapper添加完成。");
            return services;
        }

    }
}
