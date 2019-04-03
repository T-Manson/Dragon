using AutoMapper;
using Dragon.Framework.Data.Dapper;
using Dragon.Framework.Data.Dapper.Query;
using Dragon.Samples.WepApi.DBModels;
using Dragon.Samples.WepApi.DomainModels;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dragon.Samples.WepApi.Repositories
{
    public class MyTestRepository : DapperRepository<MyTest>, IMyTestRepository
    {
        private IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dapperContext"></param>
        /// <param name="loggerFactory"></param>
        public MyTestRepository(DapperContext dapperContext, IMapper mapper,
            ILoggerFactory loggerFactory = null) : base(dapperContext, loggerFactory)
        {
            this._mapper = mapper;
        }

        /// <summary>
        /// <see cref="IMyTestRepository.GetByName"/>
        /// </summary>
        public async Task<MyTestResponse> GetByName(string name)
        {
            SingleQueryFilter query = new SingleQueryFilter();
            query.AddEqual(nameof(MyTest.Name), name);
            MyTest result = await QueryFirstOrDefaultAsync(query);
            return _mapper.Map<MyTestResponse>(result);
        }
    }
}
