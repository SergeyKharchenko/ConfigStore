using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<string>> GetConfigNamesAsync(string applicationName, string environmentName, bool decrypt = true) {
            IEnumerable<string> names = await GetConfigNamesAsync();
            var configs =
                from name in names
                let configName = ConfigNameResolver.GetConfigName(applicationName, environmentName, name)
                where configName != null
                select new { ConfigName = configName, RawName = name };
            
            return decrypt 
                ? configs.Select(config => config.ConfigName) 
                : configs.Select(config => config.RawName);
        }

        private async Task<IEnumerable<string>> GetConfigNamesAsync() {
            IPage<SecretItem> secretItems = await _client.GetSecretsAsync(_keyVaultUrl);
            return secretItems.Select(item => item.Identifier.Name);
        }

        public async Task<string> GetConfigValueAsync(string applicationName, string environmentName, string configName) {
            string configNameEncrypted = ConfigNameResolver.CreateConfigName(applicationName, environmentName, configName);
            SecretBundle secretItem = await _client.GetSecretAsync(_keyVaultUrl, configNameEncrypted);
            return secretItem.Value;
        }

        public async Task AddConfigAsync(string applicationName, string environmentName, string configName, string configValue) {
            string configNameEncrypted = ConfigNameResolver.CreateConfigName(applicationName, environmentName, configName);
            await _client.SetSecretAsync(_keyVaultUrl, configNameEncrypted, configValue);
        }

        public async Task RemoveConfigsAsync(string applicationName, string environmentName) {
            IEnumerable<string> configNames = 
                await GetConfigNamesAsync(applicationName, environmentName, decrypt: false);
            await RemoveConfigsAsync(configNames);
        }

        private async Task RemoveConfigsAsync(IEnumerable<string> configNames) {
            IEnumerable<Task> removeTasks = configNames.Select(async name => await RemoveConfigAsync(name));
            await Task.WhenAll(removeTasks);
        }

        public async Task RemoveConfigAsync(string applicationName, string environmentName, string configName) {
            string configNameEncrypted = ConfigNameResolver.CreateConfigName(applicationName, environmentName, configName);
            await RemoveConfigAsync(configNameEncrypted);
        }

        private async Task RemoveConfigAsync(string configName) {
            await _client.DeleteSecretAsync(_keyVaultUrl, configName);
        }

        public async Task ClearAsync() {
            IEnumerable<string> names = await GetConfigNamesAsync();
            await RemoveConfigsAsync(names);
        }
    }
}