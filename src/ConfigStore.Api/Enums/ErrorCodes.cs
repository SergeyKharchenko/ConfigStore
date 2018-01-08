using System.ComponentModel;

namespace ConfigStore.Api.Enums
{
    public enum ErrorCodes {
        [Description("This application name is already taken")]
        ApplicationNameAleadyBusy = 1,

        [Description("This service name is already taken for this application")]
        ServiceNameAleadyBusy = 2,

        [Description("This environment name is already taken for this service")]
        EnvironmentNameAleadyBusy = 3,

        [Description("This config name was not found for this environment in this application")]
        ConfigNameNotFound = 4
    }
}
