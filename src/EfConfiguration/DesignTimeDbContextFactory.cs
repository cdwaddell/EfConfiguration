using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Titanosoft.EfConfiguration
{
    //This is a utility class that allows migrations to be setup inside of a dotnet Standard library
    internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ConfigurationContext>
    {
        public ConfigurationContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ConfigurationContext>();

            builder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CoreBlogger;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new ConfigurationContext(builder.Options);
        }
    }
}