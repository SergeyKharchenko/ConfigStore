using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest.Azure;

namespace ConfigStore.Api.Infrastructure {
    public class ConfigClient {
        private readonly KeyVaultClient _client;
        private readonly string _keyVaultUrl;

        public ConfigClient(KeyVaultClient client, IConfiguration configuration) {
            _client = client;
            _keyVaultUrl = configuration["KeyVaultUrl"];
        }

        public async Task<List<KeyValuePair<string, string>>> GetConfigsAsync() {
            List<Task<SecretBundle>> secretTasks =
                (await _client.GetSecretsAsync(_keyVaultUrl))
                .AsEnumerable()
                .Select(async secretItem => await _client.GetSecretAsync(secretItem.Id))
                .ToList();

            await Task.WhenAll(secretTasks);

            List<KeyValuePair<string, string>> keyValuePairs =
                secretTasks.Select(secret => secret.Result)
                          .Select(secret => new KeyValuePair<string, string>(secret.SecretIdentifier.Name, secret.Value))
                          .ToList();

            return keyValuePairs;
        }

        public async Task<List<KeyValuePair<string, byte[]>>> GetKeysAsync() {
            List<Task<KeyBundle>> keyTasks =
                (await _client.GetKeysAsync(_keyVaultUrl))
                .AsEnumerable()
                .Select(async keyItem => await _client.GetKeyAsync(keyItem.Identifier.Identifier))
                .ToList();

            await Task.WhenAll(keyTasks);

            List<KeyValuePair<string, byte[]>> keyValuePairs =
                keyTasks.Select(secret => secret.Result)
                       .Select(key => new KeyValuePair<string, byte[]>(key.KeyIdentifier.Name, key.Key.N))
                       .ToList();

            return keyValuePairs;
        }

        public async Task<IEnumerable<string>> GetConfigNamesAsync(string prefix) {
            IPage<SecretItem> secretItems = await _client.GetSecretsAsync(_keyVaultUrl);
            return from item in secretItems
                   let index = item.Identifier.Name.IndexOf(prefix, StringComparison.InvariantCultureIgnoreCase)
                   where index != -1
                   let configName = item.Identifier.Name.Remove(index, prefix.Length)
                   select configName;
            
        }

        public async Task<string> GetConfigValueAsync(string configName) {
            SecretBundle secretItem = await _client.GetSecretAsync(_keyVaultUrl, configName);
            return secretItem.Value;
        }

        public async Task AddConfigAsync(string configName, string configValue) {
            await _client.SetSecretAsync(_keyVaultUrl, configName, configValue);
        }
    }
}