using System;
using System.Linq;
using ConfigStore.Api.Data;
using ConfigStore.Api.Data.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConfigStore.Api.Authorization {
    public class AuthorizationEnvironmentHandler : AuthorizationApplicationHandler {
        public const string EnvironmentNameHeaderName = "CS-Environment-Key";

        public AuthorizationEnvironmentHandler(Action<Action<ConfigStoreContext>> executeActionWithContext) : base(executeActionWithContext) {
        }

        protected override ApplicationIdentity CreateIdentity(AuthorizationFilterContext filterContext) {
            Application application = GetApplication(filterContext);
            if (application == null) {
                return null;
            }
            var environmentName = (string)filterContext.HttpContext.Request.Headers[EnvironmentNameHeaderName];
            if (string.IsNullOrWhiteSpace(environmentName)) {
                return null;
            }

            ApplicationEnvironment environment = null;
            ExecuteActionWithContext(context =>
                                         environment = context.Environments.FirstOrDefault(env =>
                                                                                 env.ApplicationId == application.Id &&
                                                                                 env.Name == environmentName));
            if (environment == null) {
                return null;
            }
            return new EnvironmentIdentity(application, environment);
        }
    }
}