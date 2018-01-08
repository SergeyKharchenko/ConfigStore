using System;
using System.Linq;
using ConfigStore.Api.Data;
using ConfigStore.Api.Data.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConfigStore.Api.Authorization {
    public class AuthorizationServiceHandler : AuthorizationApplicationHandler {
        public const string ServiceKeyHeaderName = "CS-Service-Key";

        public AuthorizationServiceHandler(Action<Action<ConfigStoreContext>> executeActionWithContext) : 
            base(executeActionWithContext) {
        }

        protected override ApplicationIdentity CreateIdentity(AuthorizationFilterContext filterContext) {
            Application application = GetApplication(filterContext);
            if (application == null) {
                return null;
            }
            ApplicationService service = GetService(filterContext);
            return service == null ? null : new ServiceIdentity(application, service);
        }

        protected ApplicationService GetService(AuthorizationFilterContext filterContext) {
            if (!Guid.TryParse(filterContext.HttpContext.Request.Headers[ServiceKeyHeaderName], out Guid servKey)) {
                return null;
            }

            ApplicationService service = null;
            ExecuteActionWithContext(context =>
                                         service =
                                             context.Services
                                                    .FirstOrDefault(serv => serv.Key == servKey));
            return service;
        }
    }
}