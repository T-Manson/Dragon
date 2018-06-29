using Dragon.Samples.WepApi.Repositories;
using System.Threading.Tasks;

namespace Dragon.Samples.WepApi.Services
{
    /// <summary>
    /// MyTest 业务逻辑
    /// </summary>
    public class MyTestService : IMyTestService
    {
        /// <summary>
        /// MyTest 仓储逻辑
        /// </summary>
        private readonly IMyTestRepository _myTestRepository;

        public MyTestService(IMyTestRepository myTestRepository)
        {
            _myTestRepository = myTestRepository;
        }

        /// <summary>
        /// <see cref="IMyTestService.ExistName"/>
        /// </summary>
        public async Task<bool> ExistName(string name)
        {
            var result = await _myTestRepository.GetByName(name);
            return result != null;
        }
    }
}
