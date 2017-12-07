using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConfigStore.Api.Controllers {
    public class BaseApiController : Controller {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        protected JsonResult ToJson(object obj) {
            return Json(obj, JsonSerializerSettings);
        }
    }
}