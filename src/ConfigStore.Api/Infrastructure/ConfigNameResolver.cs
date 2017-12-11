namespace ConfigStore.Api.Infrastructure {
    public static class ConfigNameResolver {
        public static string CreatePrefix(string applicationName) {
            return $"{applicationName}-";
        }

        public static string CreateConfigName(string applicationName, string configName) {
            return $"{applicationName}-{configName}";
        }
    }
}