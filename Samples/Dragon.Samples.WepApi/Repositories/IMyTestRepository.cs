using Dragon.Framework.Core.DependencyInjection;
using Dragon.Framework.Data.Dapper.Abstractions;
using Dragon.Samples.WepApi.DBModels;
using System.Threading.Tasks;

namespace Dragon.Samples.WepApi.Repositories
{
    /// <summary>
    /// MyTest仓储接口
    /// </summary>
    public interface IMyTestRepository : IRepository<MyTest>, IDependency
    {
        /// <summary>
        /// 根据名称获取数据
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        Task<MyTest> GetByName(string name);
    }
}
