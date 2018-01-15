using ConfigStore.Api.Data;
using ConfigStore.Api.Data.Models;

namespace ConfigStore.Api.Authorization {
    public class ServiceIdentity : ApplicationIdentity {
        public ApplicationService Service { get; }

        public ServiceIdentity(Application application, ApplicationService service) : base(application) {
            Service = service;
        }
    }
}