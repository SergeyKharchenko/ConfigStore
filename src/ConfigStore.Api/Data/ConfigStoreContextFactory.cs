using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ConfigStore.Api.Data {
    public class ConfigStoreContextFactory : IDesignTimeDbContextFactory<ConfigStoreContext> {
        private const string DbConnectionStringName = "DefaultConnection";

        public ConfigStoreContext CreateDbContext(string[] args) {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString(DbConnectionStringName);
            var optionBuilder = new DbContextOptionsBuilder<ConfigStoreContext>();
            optionBuilder.UseSqlServer(connectionString);
            return new ConfigStoreContext(optionBuilder.Options);
        }
    }
}