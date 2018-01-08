using System;
using System.Collections.Generic;
using System.Linq;
using ConfigStore.Api.Authorization;
using ConfigStore.Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ConfigStore.Api.Extensions {
    public static class ControllerExtensions {
        private const int ValidationErrorStatusCode = 422;

        public static IActionResult ValidationError(this Controller controller) {
            return controller.StatusCode(ValidationErrorStatusCode, GetAllErrorMessages(controller));
        }

        private static IList<string> GetAllErrorMessages(Controller controller) {
            var errors = new List<string>();
            foreach (ModelStateEntry modelStateEntry in controller.ModelState.Values) {
                IEnumerable<string> errorMessages = from error in modelStateEntry.Errors
                                                    let message = string.IsNullOrEmpty(error.ErrorMessage)
                                                                      ? error.Exception?.Message
                                                                      : error.ErrorMessage
                                                    select message;
                errors.AddRange(errorMessages);
            }                        
            return errors;
        }

        public static Application GetApplication(this Controller controller) {
            return (controller.HttpContext.User.Identity as ApplicationIdentity)?.Application;
        }

        public static ApplicationService GetService(this Controller controller) {
            return (controller.HttpContext.User.Identity as ServiceIdentity)?.Service;
        }

        public static ServiceEnvironment GetEnvironment(this Controller controller) {
            return (controller.HttpContext.User.Identity as EnvironmentIdentity)?.Environment;
        }
    }
}