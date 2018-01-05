using System.Security.Principal;
using ConfigStore.Api.Data;
using ConfigStore.Api.Data.Models;

namespace ConfigStore.Api.Authorization {
    public class ApplicationIdentity : GenericIdentity {
        public Application Application { get; }

        public ApplicationIdentity(Application application) : base(application.Name) {
            Application = application;
        }
    }
}