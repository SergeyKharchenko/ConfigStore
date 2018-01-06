using ConfigStore.Api.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConfigStore.Api.Data {
    public class ConfigStoreContext : DbContext {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ConfigStoreContext(DbContextOptions options, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
            : base(options) {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public ConfigStoreContext() { }

        public DbSet<Application> Applications { get; set; }

        public DbSet<ApplicationEnvironment> Environments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured && _hostingEnvironment.IsEnvironment("Localhost")) {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Application>()
                   .HasIndex(app => app.Name)
                   .IsUnique();

            builder.Entity<Application>()
                   .HasMany(app => app.Environments)
                   .WithOne(env => env.Application)
                   .HasForeignKey(env => env.ApplicationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationEnvironment>()
                   .HasKey(env => new { env.ApplicationId, env.Name });
        }
    }
}