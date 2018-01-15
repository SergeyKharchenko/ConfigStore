using System;

namespace ConfigStore.Api.Exceptions {
    public class EntityNotFoundException : Exception {
        public EntityNotFoundException(string entityName, Guid key) : 
            base($"Entity ${entityName} with key: {key} not found") { }
    }
}
