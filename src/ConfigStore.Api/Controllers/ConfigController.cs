using System.Threading.Tasks;
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
            return Json(await _client.GetConfigNamesAsync(this.GetApplicationName(), this.GetEnvironmentName()));
        }

        [HttpPost("value")]
        public async Task<IActionResult> GetConfigValue(NameDto nameDto) {
            string configName = ConfigNameResolver.CreateConfigName(this.GetApplicationName(), this.GetEnvironmentName(), nameDto.Name);
            try {
                 return Json(await _client.GetConfigValueAsync(configName));
            } catch (KeyVaultErrorException) {
                return Json(ErrorDto.Create(ErrorCodes.ConfigNameNotFound));
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddConfigDto addConfigDto) {
            string configName = ConfigNameResolver.CreateConfigName(this.GetApplicationName(), this.GetEnvironmentName(), addConfigDto.ConfigName);
            await _client.AddConfigAsync(configName, addConfigDto.ConfigValue);
            return Ok();
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] NameDto nameDto) {
            string configName = ConfigNameResolver.CreateConfigName(this.GetApplicationName(), this.GetEnvironmentName(), nameDto.Name);
            await _client.RemoveConfigAsync(configName);
            return Ok();
        }

        [HttpPost("_configs")]
        public async Task<IActionResult> GetConfigs() {
            return Json(await _client.GetConfigsAsync());
        }

        [HttpPost("_keys")]
        public async Task<IActionResult> GetKeys() {
            return Json(await _client.GetKeysAsync());
        }
    }
}