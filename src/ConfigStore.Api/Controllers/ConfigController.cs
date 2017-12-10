using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigStore.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;

namespace ConfigStore.Api.Controllers {
    [Route("api/[controller]")]
    public class ConfigController : Controller {
        private readonly KeyVaultClient _client;
        private readonly IConfiguration _configuration;

        public ConfigController(KeyVaultClient client, IConfiguration configuration) {
            _client = client;
            _configuration = configuration;
        }

        [HttpGet("secrets")]
        public async Task<IActionResult> GetSecrets() {
            List<Task<SecretBundle>> ecretTasks =
                (await _client.GetSecretsAsync(_configuration["KeyVaultUrl"]))
                .AsEnumerable()
                .Select(async secretItem => await _client.GetSecretAsync(secretItem.Id))
                .ToList();

            await Task.WhenAll(ecretTasks);

            List<KeyValuePair<string, string>> keyValuePairs =
                ecretTasks.Select(secret => secret.Result)
                       .Select(secret => new KeyValuePair<string, string>(secret.SecretIdentifier.Name, secret.Value))
                       .ToList();

            return this.Json(keyValuePairs);
        }

        [HttpGet("keys")]
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

            return this.Json(keyValuePairs);
        }

        [HttpGet("{id}")]
        public string Get(int id) {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value) { }
    }
}