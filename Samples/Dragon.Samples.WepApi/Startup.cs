using Dragon.Framework.ApiCore.Extensions;
using Dragon.Framework.ApiCore.Filters;
using Dragon.Framework.ApiCore.Middlewares;
using Dragon.Framework.Core.Config;
using Dragon.Framework.Data.Dapper;
using Dragon.Framework.Mapping.Extensions;
using Dragon.Framework.Caching.Memory;
using Dragon.Framework.Caching.Redis;
using Dragon.Framework.Caching.Hybrid;
using Dragon.Framework.MessageBus.RabbitMQ;
using Dragon.Framework.MessageBus.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Dragon.Samples.WepApi
{
    /// <summary>
    /// 启动类
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 配置
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().AddNormalConfig(env.ContentRootPath, env.EnvironmentName);
            Configuration = builder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                // 启用全局Route Prefix
                options.UseGlobalRoutePrefix("api");

                // 启用Api模式
                options.UseApi();

                // 参数校验
                options.Filters.Add<WebApiActionFilterAttribute>();
            })
            // 日期格式格式化输出
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            string searchPattern = "Dragon.Samples.*.dll";

            // 添加AutoMapper支持
            services.AddAutoMapper(searchPattern);

            // 添加Dapper支持
            services.AddDapper(Configuration);

            // 添加MemoryCache支持
            services.AddMemoryCache(Configuration);

            // 添加Redis支持
            services.AddRedis(Configuration);

            // 添加HybridCache支持
            services.AddHybridCache(Configuration);

            // 添加RabbitMQ支持
            services.AddRabbitMq(Configuration);

            // 添加RedisBus支持
            services.AddRedisBus(Configuration);

            // 注册所有需要注入的服务
            services.RegisterServiceAll(searchPattern);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Information);
            loggerFactory.AddConsole(LogLevel.Warning);
            loggerFactory.AddDebug(LogLevel.Error);

            // 注入静态请求上下文
            app.UseStaticHttpContext();

            // 注入静态配置
            app.UseStaticConfiguration(Configuration);

            // 异常处理中间件
            app.UseMiddleware<ExceptionMiddleware>();

            // 启用中文编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // 跨域规则
            app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseMvc();
        }
    }
}
