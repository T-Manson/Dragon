using Dragon.Framework.ApiCore.Common;
using Dragon.Framework.Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Dragon.Samples.WepApi.Controllers
{
    [Route("[controller]")]
    public class TestController : ApiController
    {
        // GET api/values
        [HttpGet]
        public object Get()
        {
            return this.Success(new string[] { "value1", "value2" });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public object Get(int id)
        {
            var assemblies = AssemblyHelper.GetAssembliesBySearchPattern("Dragon.Samples.*.dll");
            return this.Success(PinyinHelper.GetPinyinFirst("國家Country."));
        }

        // POST api/values
        [HttpPost]
        public object Post([FromBody]string value)
        {
            return this.SuccessWithNoData();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public object Put(int id, [FromBody]string value)
        {
            return this.SuccessWithNoData();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            return this.SuccessWithNoData();
        }
    }
}
