using Microsoft.EntityFrameworkCore;

namespace ConfigStore.Api.Data {
    public class ConfigStoreContext : DbContext {
        public ConfigStoreContext(DbContextOptions options)
            : base(options) { }

        public ConfigStoreContext() { }

        public DbSet<Application> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Application>()
                   .HasIndex(app => app.Name)
                   .IsUnique();
        }
    }
}