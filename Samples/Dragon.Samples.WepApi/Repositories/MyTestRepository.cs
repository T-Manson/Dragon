using Dragon.Framework.Data.Dapper;
using Dragon.Framework.Data.Dapper.Query;
using Dragon.Samples.WepApi.DBModels;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dragon.Samples.WepApi.Repositories
{
    public class MyTestRepository : DapperRepository<MyTest>, IMyTestRepository
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dapperContext"></param>
        /// <param name="loggerFactory"></param>
        public MyTestRepository(DapperContext dapperContext, ILoggerFactory loggerFactory = null) : base(dapperContext, loggerFactory)
        {
        }

        /// <summary>
        /// <see cref="IMyTestRepository.GetByName"/>
        /// </summary>
        public async Task<MyTest> GetByName(string name)
        {
            SingleQueryFilter query = new SingleQueryFilter();
            query.AddEqual(nameof(MyTest.Name), name);
            return await QueryFirstOrDefaultAsync(query);
        }
    }
}
