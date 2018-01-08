using System;

namespace ConfigStore.Api.Infrastructure {
    public static class ConfigNameResolver {
        public static string CreateConfigName(Guid appKey, Guid servKey, Guid envKey, string configName) {
            string prefix = CreatePrefix(appKey, servKey, envKey);
            return $"{prefix}{Encryptor.Ecrypt(configName)}";
        }

        public static string DecryptConfigName(Guid appKey, Guid servKey, Guid envKey, string encryptedFullConfigName) {
            string prefix = CreatePrefix(appKey, servKey, envKey);
            if (!encryptedFullConfigName.StartsWith(prefix)) {
                return null;
            }
            string encryptedConfigName = encryptedFullConfigName.Replace(prefix, "");
            return Encryptor.Decrypt(encryptedConfigName);
        }

        private static string CreatePrefix(Guid appKey, Guid servKey, Guid envKey) {
            return Encryptor.EcryptOneWay($"{appKey}{servKey}{envKey}");
        }
    }
}