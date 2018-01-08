using System;
using System.Collections.Generic;

namespace ConfigStore.Api.Data.Models {
    public class ApplicationService {
        public int Id { get; set; }

        public Guid Key { get; set; }
        
        public string Name { get; set; }

        public virtual Application Application { get; set; }

        public int ApplicationId { get; set; }

        public virtual List<ServiceEnvironment> Environments { get; set; }
    }
}