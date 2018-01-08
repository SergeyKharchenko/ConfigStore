using System;

namespace ConfigStore.Api.Data.Models {
    public class ServiceEnvironment {
        public int Id { get; set; }

        public Guid Key { get; set; }

        public string Name { get; set; }

        public virtual ApplicationService Service { get; set; }

        public int ServiceId { get; set; }
    }
}