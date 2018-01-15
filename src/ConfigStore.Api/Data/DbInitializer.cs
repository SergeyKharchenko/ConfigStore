using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigStore.Api.Data.Models;
using ConfigStore.Api.Infrastructure;

namespace ConfigStore.Api.Data {
    public class DbInitializer {
        public static readonly Guid DefaultApplicationKey = Guid.Parse("13E139F4-4B1E-40C4-AEF9-C460FC90407B");
        public static readonly Guid DefaultServiceKey = Guid.Parse("52D56922-0D81-4375-A633-12F91212BB00");
        public static readonly Guid DefaultEnvironmentKey = Guid.Parse("0287DCA0-6EA7-4904-A243-769A492AF351");

        private readonly ConfigStoreContext _context;
        private readonly ConfigClient _client;

        public DbInitializer(ConfigStoreContext context, ConfigClient client) {
            _context = context;
            _client = client;
        }

        public async Task SeedAsync() {
            _context.Applications.RemoveRange(_context.Applications);
            await _context.SaveChangesAsync();

            Guid testEnvKey = Guid.NewGuid();
            Guid prodEnvKey = Guid.NewGuid();

            await _context.Applications.AddAsync(new Application {
                Key = DefaultApplicationKey,
                Name = "MyApp",
                Services = new List<ApplicationService> {
                    new ApplicationService {
                        Key = DefaultServiceKey,
                        Name = "Calculation",
                        Environments = new List<ServiceEnvironment> {
                            new ServiceEnvironment { Key = DefaultEnvironmentKey, Name = "Development" },
                            new ServiceEnvironment { Key = testEnvKey, Name = "Test" },
                            new ServiceEnvironment { Key = prodEnvKey, Name = "Production" }
                        }
                    }
                }
            });

            await _client.AddConfigAsync(DefaultApplicationKey, DefaultServiceKey, DefaultEnvironmentKey, "MyConnectionString", "Data Source=UA01WP0257;Initial Catalog=ConfigStorage;Integrated Security=True;MultipleActiveResultSets=True");
            await _client.AddConfigAsync(DefaultApplicationKey, DefaultServiceKey, DefaultEnvironmentKey, "ASPNETCORE_ENVIRONMENT", "Development");

            await _client.AddConfigAsync(DefaultApplicationKey, DefaultServiceKey, testEnvKey, "ASPNETCORE_ENVIRONMENT", "Test");

            await _client.AddConfigAsync(DefaultApplicationKey, DefaultServiceKey, prodEnvKey, "ASPNETCORE_ENVIRONMENT", "Production");

            await _context.SaveChangesAsync();
        }
    }
}