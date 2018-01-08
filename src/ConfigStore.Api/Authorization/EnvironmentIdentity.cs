using ConfigStore.Api.Data.Models;

namespace ConfigStore.Api.Authorization {
    public class EnvironmentIdentity : ServiceIdentity {
        public ServiceEnvironment Environment { get; }

        public EnvironmentIdentity(Application application, ApplicationService service, ServiceEnvironment environment) 
            : base(application, service) {
            Environment = environment;
        }
    }
}