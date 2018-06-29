using Dragon.Framework.ApiCore.Common;
using Dragon.Framework.Infrastructure.Helpers;
using Dragon.Samples.WepApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dragon.Samples.WepApi.Controllers
{
    /// <summary>
    /// MyTest 控制器
    /// </summary>
    [Route("[controller]")]
    public class MyTestController : ApiController
    {
        /// <summary>
        /// MyTest 业务逻辑
        /// </summary>
        private readonly IMyTestService _myTestService;

        public MyTestController(IMyTestService myTestService)
        {
            _myTestService = myTestService;
        }

        [HttpGet("exist-name/{name}")]
        public async Task<object> ExistName(string name)
        {
            var result = await _myTestService.ExistName(name);
            return Success(result);
        }

        // GET api/test
        [HttpGet]
        public object Get()
        {
            return Success(new string[] { "value1", "value2" });
        }

        // GET api/test/5
        [HttpGet("{id}")]
        public object Get(int id)
        {
            var assemblies = AssemblyHelper.GetAssembliesBySearchPattern("Dragon.Samples.*.dll");
            return Success(PinyinHelper.GetPinyinFirst("國家Country."));
        }

        // POST api/test
        [HttpPost]
        public object Post([FromBody]string value)
        {
            return SuccessWithNoData();
        }

        // PUT api/test/5
        [HttpPut("{id}")]
        public object Put(int id, [FromBody]string value)
        {
            return SuccessWithNoData();
        }

        // DELETE api/test/5
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            return SuccessWithNoData();
        }
    }
}
