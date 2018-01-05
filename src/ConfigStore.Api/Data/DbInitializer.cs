using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigStore.Api.Data.Models;
using ConfigStore.Api.Infrastructure;

namespace ConfigStore.Api.Data {
    public class DbInitializer {
        public static readonly Guid DefaultAppKey = Guid.Parse("13E139F4-4B1E-40C4-AEF9-C460FC90407B");

        private readonly ConfigStoreContext _context;
        private readonly ConfigClient _client;

        public DbInitializer(ConfigStoreContext context, ConfigClient client) {
            _context = context;
            _client = client;
        }

        public async Task SeedAsync() {
            await _context.Applications.AddAsync(new Application {
                Name = "myapp",
                Key = DefaultAppKey,
                Environments = new List<ApplicationEnvironment> {
                    new ApplicationEnvironment { Name = "development" },
                    new ApplicationEnvironment { Name = "test" },
                    new ApplicationEnvironment { Name = "production" }
                }
            });

            await _client.AddConfigAsync("myapp", "development", "MyConnectionString", "Data Source=UA01WP0257;Initial Catalog=ConfigStorage;Integrated Security=True;MultipleActiveResultSets=True");
            await _client.AddConfigAsync("myapp", "development", "ASPNETCORE_ENVIRONMENT", "Development");

            await _client.AddConfigAsync("myapp", "test", "ASPNETCORE_ENVIRONMENT", "Test");

            await _client.AddConfigAsync("myapp", "production", "ASPNETCORE_ENVIRONMENT", "Production");

            await _context.SaveChangesAsync();
        }
    }
}