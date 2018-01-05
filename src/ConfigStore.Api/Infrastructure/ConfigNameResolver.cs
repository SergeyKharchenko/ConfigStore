namespace ConfigStore.Api.Infrastructure {
    public static class ConfigNameResolver {
        public const string Separator = "-";

        public static string CreatePrefix(string applicationName, string evironmentName) {
            return Encryptor.EcryptOneWay($"{applicationName}{Separator}{evironmentName}{Separator}");
        }

        public static string CreateConfigName(string applicationName, string evironmentName, string configName) {
            string prefix = CreatePrefix(applicationName, evironmentName);
            return $"{prefix}{Encryptor.Ecrypt(configName)}";
        }

        public static string GetConfigName(string applicationName, string evironmentName, string encryptedFullConfigName) {
            string prefix = CreatePrefix(applicationName, evironmentName);
            if (!encryptedFullConfigName.StartsWith(prefix)) {
                return null;
            }
            string encryptedConfigName = encryptedFullConfigName.Replace(prefix, "");
            return Encryptor.Decrypt(encryptedConfigName);
        }
    }
}