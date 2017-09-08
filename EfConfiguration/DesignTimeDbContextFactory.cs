using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Titanosoft.EfConfiguration
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ConfigurationContext>
    {
        public ConfigurationContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ConfigurationContext>();

            var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CoreBlogger;Trusted_Connection=True;MultipleActiveResultSets=true";

            builder.UseSqlServer(connectionString);

            return new ConfigurationContext(builder.Options);
        }
    }
}