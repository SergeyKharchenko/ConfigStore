using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigStore.Api.Data.Models;

namespace ConfigStore.Api.Infrastructure {
    public class DefaultDataInitializer {
        private readonly ConfigClient _client;

        public DefaultDataInitializer(ConfigClient client) {
            _client = client;
        }

        public async Task<List<ApplicationService>> CreateDefaultServices(Guid appKey) {
            Guid servKey = Guid.NewGuid();
            return new List<ApplicationService> {
                new ApplicationService {
                    Key = servKey,
                    Name = "MyService",
                    Environments = await CreateDefaultEnvironments(appKey, servKey) 
                }
            };
        }

        public async Task<List<ServiceEnvironment>> CreateDefaultEnvironments(Guid appKey, Guid servKey) {
            Guid envKey = Guid.NewGuid();
            await CreateDefaultConfig(appKey, servKey, envKey);
            return new List<ServiceEnvironment> {
                new ServiceEnvironment {
                    Key = envKey,
                    Name = "Development"
                }
            };
        }

        public async Task CreateDefaultConfig(Guid appKey, Guid servKey, Guid envKey) {
            await _client.AddConfigAsync(appKey, servKey, envKey, "Greetings", "Hello World!");
        }
    }
}