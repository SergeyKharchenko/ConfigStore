using System;
using System.Linq;
using ConfigStore.Api.Data;
using ConfigStore.Api.Data.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConfigStore.Api.Authorization {
    public class AuthorizationEnvironmentHandler : AuthorizationServiceHandler {
        public const string EnvironmentKeyHeaderName = "CS-Environment-Key";

        public AuthorizationEnvironmentHandler(Action<Action<ConfigStoreContext>> executeActionWithContext) : 
            base(executeActionWithContext) { }

        protected override ApplicationIdentity CreateIdentity(AuthorizationFilterContext filterContext) {
            Application application = GetApplication(filterContext);
            if (application == null) {
                return null;
            }
            ApplicationService service = GetService(filterContext);
            if (service == null) {
                return null;
            }
            ServiceEnvironment environment = GetEnvironment(filterContext);
            return environment == null ? null : new EnvironmentIdentity(application, service, environment);
        }

        protected ServiceEnvironment GetEnvironment(AuthorizationFilterContext filterContext) {
            if (!Guid.TryParse(filterContext.HttpContext.Request.Headers[EnvironmentKeyHeaderName], out Guid envKey)) {
                return null;
            }

            ServiceEnvironment service = null;
            ExecuteActionWithContext(context =>
                                         service =
                                             context.Environments
                                                    .FirstOrDefault(env => env.Key == envKey));
            return service;
        }
    }
}