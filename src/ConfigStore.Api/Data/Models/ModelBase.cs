using System;

namespace ConfigStore.Api.Data.Models {
    public class ModelBase {
        public int Id { get; set; }

        public Guid Key { get; set; }

        public string Name { get; set; }
    }
}