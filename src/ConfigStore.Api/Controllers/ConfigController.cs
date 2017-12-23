using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigStore.Api.Dto.Input;
using ConfigStore.Api.Dto.Output;
using ConfigStore.Api.Enums;
using ConfigStore.Api.Extensions;
using ConfigStore.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest.Azure;

namespace ConfigStore.Api.Controllers {
    [Route("api/[controller]")]
    [Authorize("environment")]
    public class ConfigController : Controller {
        private readonly ConfigClient _client;

        public ConfigController(ConfigClient client) {
            _client = client;
        }

        [HttpPost("getApplicationConfigNames")]
        public async Task<IActionResult> GetApplicationConfigNames() {
            string prefix = $"{ConfigNameResolver.CreatePrefix(this.GetApplicationName(), this.GetEnvironment().Name)}{ConfigNameResolver.Separator}";
            return Json(await _client.GetConfigNamesAsync(prefix));
        }

        [HttpPost("getApplicationConfig")]
        public async Task<IActionResult> GetApplicationConfig(NameDto nameDto) {
            string configName = ConfigNameResolver.CreateConfigName(this.GetApplicationName(), this.GetEnvironment().Name, nameDto.Name);
            try {
                 return Json(await _client.GetConfigValueAsync(configName));
            } catch (KeyVaultErrorException) {
                return Json(ErrorDto.Create(ErrorCodes.ConfigNameNotFound));
            }
        }

        [HttpPost("addApplicationSecrets")]
        public async Task<IActionResult> AddApplicationSecret([FromBody] AddConfigDto addConfigDto) {
            string configName = ConfigNameResolver.CreateConfigName(this.GetApplicationName(), this.GetEnvironment().Name, addConfigDto.ConfigName);
            await _client.AddConfigAsync(configName, addConfigDto.ConfigValue);
            return Ok();
        }

        [HttpPost("configs")]
        public async Task<IActionResult> GetConfigs() {
            return Json(await _client.GetConfigsAsync());
        }

        [HttpPost("keys")]
        public async Task<IActionResult> GetKeys() {
            return Json(await _client.GetKeysAsync());
        }
    }
}