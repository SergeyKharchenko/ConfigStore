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
        private readonly KeyVaultClient _client;
        private readonly IConfiguration _configuration;

        public ConfigController(KeyVaultClient client, IConfiguration configuration) {
            _client = client;
            _configuration = configuration;
        }

        [HttpPost("addApplicationSecrets")]
        public async Task<IActionResult> AddApplicationSecret([FromBody] AddConfigDto addConfigDto) {
            string configName = ConfigNameResolver.CreateConfigName(this.GetApplicationName(), this.GetEnvironment().Name, addConfigDto.ConfigName);
            await _client.SetSecretAsync(_configuration["KeyVaultUrl"], configName, addConfigDto.ConfigValue);
            return Ok();
        }

        [HttpPost("applicationSecrets")]
        public async Task<IActionResult> GetApplicationSecrets() {
            IPage<SecretItem> secretItems = await _client.GetSecretsAsync(_configuration["KeyVaultUrl"]);

            string prefix = $"{ConfigNameResolver.CreatePrefix(this.GetApplicationName(), this.GetEnvironment().Name)}{ConfigNameResolver.Separator}";

            IEnumerable<string> secretNames =
                from item in secretItems
                let index = item.Identifier.Name.IndexOf(prefix, StringComparison.InvariantCultureIgnoreCase)
                where index != -1
                let configName = item.Identifier.Name.Remove(index, prefix.Length)
                select configName;

            return Json(secretNames);
        }

        [HttpPost("applicationSecret")]
        public async Task<IActionResult> GetApplicationSecret(NameDto nameDto) {
            string configName = ConfigNameResolver.CreateConfigName(this.GetApplicationName(), this.GetEnvironment().Name, nameDto.Name);
            try {
                SecretBundle secretItem = await _client.GetSecretAsync(_configuration["KeyVaultUrl"], configName);
                 return Json(secretItem.Value);
            } catch (KeyVaultErrorException) {
                return Json(ErrorDto.Create(ErrorCodes.ConfigNameNotFound));
            }
        }






        [HttpPost("secrets")]
        public async Task<IActionResult> GetSecrets() {
            List<Task<SecretBundle>> secretTasks =
                (await _client.GetSecretsAsync(_configuration["KeyVaultUrl"]))
                .AsEnumerable()
                .Select(async secretItem => await _client.GetSecretAsync(secretItem.Id))
                .ToList();

            await Task.WhenAll(secretTasks);

            List<KeyValuePair<string, string>> keyValuePairs =
                secretTasks.Select(secret => secret.Result)
                          .Select(secret => new KeyValuePair<string, string>(secret.SecretIdentifier.Name, secret.Value))
                          .ToList();

            return Json(keyValuePairs);
        }

        [HttpPost("keys")]
        public async Task<IActionResult> GetKeys() {
            List<Task<KeyBundle>> keyTasks =
                (await _client.GetKeysAsync(_configuration["KeyVaultUrl"]))
                .AsEnumerable()
                .Select(async keyItem => await _client.GetKeyAsync(keyItem.Identifier.Identifier))
                .ToList();

            await Task.WhenAll(keyTasks);

            List<KeyValuePair<string, byte[]>> keyValuePairs =
                keyTasks.Select(secret => secret.Result)
                       .Select(key => new KeyValuePair<string, byte[]>(key.KeyIdentifier.Name, key.Key.N))
                       .ToList();

            return Json(keyValuePairs);
        }
    }
}