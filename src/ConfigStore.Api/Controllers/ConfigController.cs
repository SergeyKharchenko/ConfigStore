using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ConfigStore.Api.Controllers {
    [Route("api/[controller]")]
    public class ConfigController : Controller {
        [HttpGet]
        public IEnumerable<string> Get() {
            return new[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public string Get(int id) {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value) { }
    }
}