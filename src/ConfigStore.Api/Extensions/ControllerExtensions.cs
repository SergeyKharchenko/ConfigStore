using System.Collections.Generic;
using System.Linq;
using ConfigStore.Api.Authorization;
using ConfigStore.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        public static string GetApplicationName(this Controller controller) {
            return controller.HttpContext.User.Identity.Name;
        }

        public static Application GetApplication(this Controller controller) {
            return (controller.HttpContext.User.Identity as ApplicationIdentity)?.Application;
        }

        public static ApplicationEnvironment GetEnvironment(this Controller controller) {
            return (controller.HttpContext.User.Identity as EnvironmentIdentity)?.Environment;
        }
    }
}