using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using ConfigStore.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ConfigStore.Api.Authorization {
    public class AuthorizationHandler : AuthorizationHandler<AuthorizationHandler>, IAuthorizationRequirement {
        public const string ApplicationKeyHeaderName = "CS-Application-Key";
        private readonly Func<Func<ConfigStoreContext, Application>, Application> _executeActionWithContext;

        public AuthorizationHandler(Func<Func<ConfigStoreContext, Application>, Application> executeActionWithContext) {
            _executeActionWithContext = executeActionWithContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationHandler requirement) {
            if (!(context.Resource is AuthorizationFilterContext filterContext)) {
                return Task.CompletedTask;
            }

            var applicationKey = (string)filterContext.HttpContext.Request.Headers[ApplicationKeyHeaderName];
            if (string.IsNullOrWhiteSpace(applicationKey)) {
                return Task.CompletedTask;
            }

            if (!Guid.TryParse(applicationKey, out Guid key)) {
                return Task.CompletedTask;
            }

            Application application = _executeActionWithContext(dbContext => 
                dbContext.Applications.FirstOrDefault(app => Equals(app.Key, key)));
            if (application == null) {
                return Task.CompletedTask;
            }

            filterContext.HttpContext.User = new GenericPrincipal(new GenericIdentity(application.Name), new string[0]);

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}