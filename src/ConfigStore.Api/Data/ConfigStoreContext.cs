using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConfigStore.Api.Data {
    public class ConfigStoreContext : DbContext {
        private readonly IConfiguration Configuration;

        public ConfigStoreContext(DbContextOptions options, IConfiguration configuration)
            : base(options) {
            Configuration = configuration;
        }

        public ConfigStoreContext() { }

        public DbSet<Application> Applications { get; set; }

        public DbSet<ApplicationEnvironment> Environments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Application>()
                   .HasIndex(app => app.Name)
                   .IsUnique();

            builder.Entity<Application>()
                   .HasMany(app => app.Environments)
                   .WithOne(env => env.Application)
                   .HasForeignKey(env => env.ApplicationId);

            builder.Entity<ApplicationEnvironment>()
                   .HasKey(env => new { env.ApplicationId, env.Name });
        }
    }
}