using Dragon.Framework.Data.Dapper.Metadata;
using Dragon.Samples.WepApi.DBModels;

namespace Dragon.Samples.WepApi.DBMappings
{
    /// <summary>
    /// 模型映射
    /// </summary>
    public class MyTestMapping : DapperMetadataProvider<MyTest>
    {
        /// <summary>
        /// 模型配置
        /// </summary>
        /// <param name="builder"></param>
        protected override void ConfigureModel(DapperMetadataBuilder<MyTest> builder)
        {
            builder.SetTableName("mytest");
            builder.SetPrimaryKey(model => model.Id);
        }
    }
}
