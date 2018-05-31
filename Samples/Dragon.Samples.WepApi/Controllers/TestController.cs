using Dragon.Framework.ApiCore.Common;
using Dragon.Framework.Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Dragon.Samples.WepApi.Controllers
{
    [Route("[controller]")]
    public class TestController : ApiController
    {
        // GET api/test
        [HttpGet]
        public object Get()
        {
            return this.Success(new string[] { "value1", "value2" });
        }

        // GET api/test/5
        [HttpGet("{id}")]
        public object Get(int id)
        {
            var assemblies = AssemblyHelper.GetAssembliesBySearchPattern("Dragon.Samples.*.dll");
            return this.Success(PinyinHelper.GetPinyinFirst("國家Country."));
        }

        // POST api/test
        [HttpPost]
        public object Post([FromBody]string value)
        {
            return this.SuccessWithNoData();
        }

        // PUT api/test/5
        [HttpPut("{id}")]
        public object Put(int id, [FromBody]string value)
        {
            return this.SuccessWithNoData();
        }

        // DELETE api/test/5
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            return this.SuccessWithNoData();
        }
    }
}
