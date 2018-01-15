using System;
using System.Collections.Generic;

namespace ConfigStore.Api.Data.Models {
    public class Application : ModelBase {
        public virtual List<ApplicationService> Services { get; set; }
    }
}