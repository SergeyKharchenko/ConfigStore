using ConfigStore.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigStore.Api.Data {
    public class ConfigStoreContext : DbContext {
        public DbSet<Application> Applications { get; set; }

        public DbSet<ApplicationEnvironment> Environments { get; set; }

        public ConfigStoreContext(DbContextOptions options)
            : base(options) {
        }

        public ConfigStoreContext() { }

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