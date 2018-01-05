using System;
using System.Collections.Generic;

namespace ConfigStore.Api.Data.Models {
    public class Application {
        public int Id { get; set; }

        public Guid Key { get; set; }

        public string Name { get; set; }

        public virtual List<ApplicationEnvironment> Environments { get; set; }
    }
}