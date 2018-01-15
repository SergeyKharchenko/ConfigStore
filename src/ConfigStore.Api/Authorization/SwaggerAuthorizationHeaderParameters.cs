using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigStore.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ConfigStore.Api.Authorization {
    public class SwaggerAuthorizationHeaderParameters : IOperationFilter {
        public void Apply(Operation operation, OperationFilterContext context) {
            if (operation.Parameters == null) {
                operation.Parameters = new List<IParameter>();
            }

            if (ShouldAddHeadder(context?.ApiDescription?.ActionDescriptor, "application", "service", "environment")) {
                operation.Parameters.Add(new NonBodyParameter {
                    Name = AuthorizationApplicationHandler.ApplicationKeyHeaderName,
                    In = "header",
                    Description = "Key of application",
                    Required = true,
                    Type = typeof(Guid).FullName,
                    Default = DbInitializer.DefaultApplicationKey
                });
            }

            if (ShouldAddHeadder(context?.ApiDescription?.ActionDescriptor, "service", "environment")) {
                operation.Parameters.Add(new NonBodyParameter {
                    Name = AuthorizationServiceHandler.ServiceKeyHeaderName,
                    In = "header",
                    Description = "Key of service",
                    Required = true,
                    Type = typeof(Guid).FullName,
                    Default = DbInitializer.DefaultServiceKey
                });
            }

            if (ShouldAddHeadder(context?.ApiDescription?.ActionDescriptor, "environment")) {
                operation.Parameters.Add(new NonBodyParameter {
                    Name = AuthorizationEnvironmentHandler.EnvironmentKeyHeaderName,
                    In = "header",
                    Description = "Key of environment",
                    Required = true,
                    Type = typeof(Guid).FullName,
                    Default = DbInitializer.DefaultEnvironmentKey
                });
            }
        }

        internal static bool ShouldAddHeadder(ActionDescriptor actionDescriptor, params string[] authorizationPolicies) {
            if (!(actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)) {
                return false;
            }
            var customAttributes = new List<CustomAttributeData>(controllerActionDescriptor.ControllerTypeInfo.CustomAttributes);
            customAttributes.AddRange(controllerActionDescriptor.MethodInfo.CustomAttributes);
            CustomAttributeData attributeData = customAttributes.FirstOrDefault(attr => attr.AttributeType == typeof(AuthorizeAttribute));
            if (attributeData == null) {
                return false;
            }
            return attributeData.ConstructorArguments.Select(arg => arg.Value.ToString())
                         .Any(authorizationPolicies.Contains);
        }
    }
}