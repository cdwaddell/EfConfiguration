using Microsoft.EntityFrameworkCore;

namespace EfConfiguration
{
    public class ConfigurationContext : DbContext
    {
        public ConfigurationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ConfigurationValue> Values { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ConfigurationValue>(t =>
            {
                t.HasKey(x => x.Key);

                t.Property(x => x.Key)
                    .HasMaxLength(64);

                t.HasIndex(x => x.LastUpdated);

                t.ToTable("ConfigurationValues", "cfg");
            });
        }
    }
}