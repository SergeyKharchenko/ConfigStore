using System;

namespace ConfigStore.Api.Data {
    public class Application {
        public int Id { get; set; }

        public Guid Key { get; set; }

        public string Name { get; set; }
    }
}