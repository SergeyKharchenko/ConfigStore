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
            if (!IsAuthorizationApplied(context?.ApiDescription?.ActionDescriptor)) {
                return;
            }
            if (operation.Parameters == null) {
                operation.Parameters = new List<IParameter>();
            }
            operation.Parameters.Add(new NonBodyParameter {
                Name = AuthorizationHandler.ApplicationKeyHeaderName,
                In = "header",
                Description = "Key for registered application",
                Required = true,
                Type = "string",
                Default = Guid.Empty
            });
        }

        internal static bool IsAuthorizationApplied(ActionDescriptor actionDescriptor) {
            if (!(actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)) {
                return false;
            }
            var customAttributes = new List<CustomAttributeData>(controllerActionDescriptor.ControllerTypeInfo.CustomAttributes);
            customAttributes.AddRange(controllerActionDescriptor.MethodInfo.CustomAttributes);
            return customAttributes.All(attr => attr.AttributeType != typeof(AllowAnonymousAttribute));
        }
    }
}