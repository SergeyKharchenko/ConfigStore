using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigStore.Api.Enums
{
    public enum ErrorCodes {
        [Description("This application name is already taken")]
        ApplicationNameAleadyBusy = 1
    }
}
