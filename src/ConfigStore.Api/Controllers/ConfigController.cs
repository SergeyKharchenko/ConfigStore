using System.Threading.Tasks;
using ConfigStore.Api.Data.Models;
using ConfigStore.Api.Dto.Input;
using ConfigStore.Api.Dto.Output;
using ConfigStore.Api.Enums;
using ConfigStore.Api.Extensions;
using ConfigStore.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault.Models;

namespace ConfigStore.Api.Controllers {
    [Route("api/[controller]")]
    [Authorize("environment")]
    public class ConfigController : Controller {
        private readonly ConfigClient _client;

        public ConfigController(ConfigClient client) {
            _client = client;
        }

        [HttpPost("names")]
        public async Task<IActionResult> GetNames() {
            Application app = this.GetApplication();
            ApplicationService serv = this.GetService();
            ServiceEnvironment env = this.GetEnvironment();
            return Json(await _client.GetConfigNamesAsync(app.Key, serv.Key, env.Key));
        }

        [HttpPost("value")]
        public async Task<IActionResult> GetValue(NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            Application app = this.GetApplication();
            ApplicationService serv = this.GetService();
            ServiceEnvironment env = this.GetEnvironment();
            try {
                return Json(await _client.GetConfigValueAsync(app.Key, serv.Key, env.Key, nameDto.Name));
            } catch (KeyVaultErrorException) {
                return Json(ErrorDto.Create(ErrorCodes.ConfigNameNotFound));
            }
        }

        [HttpPost("addOrUpdate")]
        public async Task<IActionResult> AddOrUpdate([FromBody] AddConfigDto addConfigDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            Application app = this.GetApplication();
            ApplicationService serv = this.GetService();
            ServiceEnvironment env = this.GetEnvironment();
            await _client.AddConfigAsync(app.Key, serv.Key, env.Key, addConfigDto.ConfigName, addConfigDto.ConfigValue);
            return Ok();
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            Application app = this.GetApplication();
            ApplicationService serv = this.GetService();
            ServiceEnvironment env = this.GetEnvironment();
            try {
                await _client.RemoveConfigAsync(app.Key, serv.Key, env.Key, nameDto.Name);
            } catch (KeyVaultErrorException) {
                return Json(ErrorDto.Create(ErrorCodes.ConfigNameNotFound));
            }
            return Ok();
        }
    }
}