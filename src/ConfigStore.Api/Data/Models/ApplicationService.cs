using System;
using System.Collections.Generic;

namespace ConfigStore.Api.Data.Models {
    public class ApplicationService : ModelBase {
        public virtual Application Application { get; set; }

        public int ApplicationId { get; set; }

        public virtual List<ServiceEnvironment> Environments { get; set; }
    }
}