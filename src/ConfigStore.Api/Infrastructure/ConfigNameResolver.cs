namespace ConfigStore.Api.Infrastructure {
    public static class ConfigNameResolver {
        public const string Separator = "-";

        public static string CreatePrefix(string applicationName, string evironmentName) {
            return Encryptor.Ecrypt($"{applicationName}{Separator}{evironmentName}");
        }

        public static string CreateConfigName(string applicationName, string evironmentName, string configName) {
            string prefix = CreatePrefix(applicationName, evironmentName);
            return $"{prefix}{Separator}{configName}";
        }
    }
}