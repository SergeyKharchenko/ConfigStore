using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;

namespace ConfigStore.Api.Infrastructure {
    public class ConfigClient {
        private readonly KeyVaultClient _client;
        private readonly string _keyVaultUrl;

        public ConfigClient(KeyVaultClient client, IConfiguration configuration) {
            _client = client;
            _keyVaultUrl = configuration["KeyVaultUrl"];
        }

        public async Task<IEnumerable<(string Name, string Value)>> GetConfigsAsync(Guid appKey, Guid servKey, Guid envKey) {
            IEnumerable<SecretItem> secretItems = await GetSecretItemsAsync(appKey, servKey, envKey);
            List<Task<(string Name, string Value)>> valueTasks = secretItems.Select(async item => (
                ConfigNameResolver.DecryptConfigName(appKey, servKey, envKey, item.Identifier.Name),
                (await _client.GetSecretAsync(_keyVaultUrl, item.Identifier.Name)).Value
            )).ToList();
            await Task.WhenAll(valueTasks);
            return valueTasks.Select(task => (task.Result.Name, task.Result.Value));
        }

        public async Task<IEnumerable<string>> GetConfigNamesAsync(Guid appKey, Guid servKey, Guid envKey, bool decrypt = true) {
            IEnumerable<SecretItem> secretItems = await GetSecretItemsAsync(appKey, servKey, envKey);
            return decrypt 
                ? secretItems.Select(item => ConfigNameResolver.DecryptConfigName(appKey, servKey, envKey, item.Identifier.Name)) 
                : secretItems.Select(item => item.Identifier.Name);
        }

        private async Task<IEnumerable<SecretItem>> GetSecretItemsAsync(Guid appKey, Guid servKey, Guid envKey) {
            IEnumerable<SecretItem> secretItems = await GetSecretItemsAsync();
            return from item in secretItems
                   let configName = ConfigNameResolver.DecryptConfigName(appKey, servKey, envKey, item.Identifier.Name)
                   where configName != null
                   select item;
        }

        private async Task<IEnumerable<SecretItem>> GetSecretItemsAsync() {
            return await _client.GetSecretsAsync(_keyVaultUrl);
        }

        public async Task<string> GetConfigValueAsync(Guid appKey, Guid servKey, Guid envKey, string configName) {
            string configNameEncrypted = ConfigNameResolver.CreateConfigName(appKey, servKey, envKey, configName);
            SecretBundle secretItem = await _client.GetSecretAsync(_keyVaultUrl, configNameEncrypted);
            return secretItem.Value;
        }

        public async Task AddConfigAsync(Guid appKey, Guid servKey, Guid envKey, string configName, string configValue) {
            string configNameEncrypted = ConfigNameResolver.CreateConfigName(appKey, servKey, envKey, configName);
            await _client.SetSecretAsync(_keyVaultUrl, configNameEncrypted, configValue);
        }

        public async Task RemoveConfigsAsync(Guid appKey, Guid servKey, Guid envKey) {
            IEnumerable<string> configNames = 
                await GetConfigNamesAsync(appKey, servKey, envKey, decrypt: false);
            await RemoveConfigsAsync(configNames);
        }

        private async Task RemoveConfigsAsync(IEnumerable<string> configNames) {
            IEnumerable<Task> removeTasks = configNames.Select(async name => await RemoveConfigAsync(name));
            await Task.WhenAll(removeTasks);
        }

        public async Task RemoveConfigAsync(Guid appKey, Guid servKey, Guid envKey, string configName) {
            string configNameEncrypted = ConfigNameResolver.CreateConfigName(appKey, servKey, envKey, configName);
            await RemoveConfigAsync(configNameEncrypted);
        }

        private async Task RemoveConfigAsync(string configName) {
            await _client.DeleteSecretAsync(_keyVaultUrl, configName);
        }

        public async Task ClearAsync() {
            IEnumerable<SecretItem> configs = await GetSecretItemsAsync();
            await RemoveConfigsAsync(configs.Select(config => config.Identifier.Name));
        }






        public async Task<List<KeyValuePair<string, byte[]>>> __GetKeysAsync() {
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
    }
}