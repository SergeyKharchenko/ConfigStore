using ConfigStore.Api.Data;

namespace ConfigStore.Api.Authorization {
    public class EnvironmentIdentity : ApplicationIdentity {
        public ApplicationEnvironment Environment { get; }

        public EnvironmentIdentity(Application application, ApplicationEnvironment environment) : base(application) {
            Environment = environment;
        }
    }
}