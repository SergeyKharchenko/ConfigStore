using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            if (ShouldAddHeadder(context?.ApiDescription?.ActionDescriptor, "application", "environment")) {
                operation.Parameters.Add(new NonBodyParameter {
                    Name = AuthorizationApplicationHandler.ApplicationKeyHeaderName,
                    In = "header",
                    Description = "Key for registered application",
                    Required = true,
                    Type = "string",
                    Default = Guid.Empty
                });
            }

            if (ShouldAddHeadder(context?.ApiDescription?.ActionDescriptor, "environment")) {
                operation.Parameters.Add(new NonBodyParameter {
                    Name = AuthorizationEnvironmentHandler.EnvironmentNameHeaderName,
                    In = "header",
                    Description = "Name of environment",
                    Required = true,
                    Type = "string",
                    Default = "Development"
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