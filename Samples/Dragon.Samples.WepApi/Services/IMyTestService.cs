using Dragon.Framework.Core.DependencyInjection;
using System.Threading.Tasks;

namespace Dragon.Samples.WepApi.Services
{
    /// <summary>
    /// MyTest 业务逻辑接口
    /// </summary>
    public interface IMyTestService : IDependency
    {
        /// <summary>
        /// 名称是否存在
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        Task<bool> ExistName(string name);
    }
}
