using ConfigStore.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigStore.Api.Data {
    public class ConfigStoreContext : DbContext {
        public DbSet<Application> Applications { get; set; }

        public DbSet<ApplicationService> Services { get; set; }

        public DbSet<ServiceEnvironment> Environments { get; set; }

        public ConfigStoreContext(DbContextOptions options)
            : base(options) {
        }

        public ConfigStoreContext() { }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Application>()
                   .HasIndex(app => app.Key)
                   .IsUnique();
            builder.Entity<Application>()
                   .HasIndex(app => app.Name)
                   .IsUnique();

            builder.Entity<ApplicationService>()
                   .HasIndex(serv => serv.Key)
                   .IsUnique();
            builder.Entity<ApplicationService>()
                   .HasIndex(serv => new { serv.ApplicationId, serv.Name })
                   .IsUnique();

            builder.Entity<ServiceEnvironment>()
                   .HasIndex(env => env.Key)
                   .IsUnique();
            builder.Entity<ServiceEnvironment>()
                   .HasIndex(env => new { env.ServiceId, env.Name })
                   .IsUnique();

            builder.Entity<Application>()
                   .HasMany(app => app.Services)
                   .WithOne(env => env.Application)
                   .HasForeignKey(env => env.ApplicationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationService>()
                   .HasMany(app => app.Environments)
                   .WithOne(env => env.Service)
                   .HasForeignKey(env => env.ServiceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}