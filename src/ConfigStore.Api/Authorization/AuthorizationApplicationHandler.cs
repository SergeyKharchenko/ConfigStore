using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using ConfigStore.Api.Data;
using ConfigStore.Api.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConfigStore.Api.Authorization {
    public class AuthorizationApplicationHandler : AuthorizationHandler<AuthorizationApplicationHandler>, IAuthorizationRequirement {
        public const string ApplicationKeyHeaderName = "CS-Application-Key";

        protected Action<Action<ConfigStoreContext>> ExecuteActionWithContext { get; }

        public AuthorizationApplicationHandler(Action<Action<ConfigStoreContext>> executeActionWithContext) {
            ExecuteActionWithContext = executeActionWithContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationApplicationHandler requirement) {
            if (!(context.Resource is AuthorizationFilterContext filterContext)) {
                return Task.CompletedTask;
            }
            ApplicationIdentity identity = CreateIdentity(filterContext);
            if (identity != null) {
                filterContext.HttpContext.User = new GenericPrincipal(identity, new string[0]);
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

        protected virtual ApplicationIdentity CreateIdentity(AuthorizationFilterContext filterContext) {
            Application application = GetApplication(filterContext);
            return application == null ? null : new ApplicationIdentity(application);
        }

        protected Application GetApplication(AuthorizationFilterContext filterContext) {
            var applicationKey = (string)filterContext.HttpContext.Request.Headers[ApplicationKeyHeaderName];
            if (string.IsNullOrWhiteSpace(applicationKey)) {
                return null;
            }
            if (!Guid.TryParse(applicationKey, out Guid key)) {
                return null;
            }
            Application application = null;
            ExecuteActionWithContext(dbContext => application = dbContext.Applications.FirstOrDefault(app => app.Key == key));
            return application;
        }
    }
}