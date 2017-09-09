using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Titanosoft.EfConfiguration
{
    public static class EfConfigurationExtensions
    {
        public static IConfigurationBuilder AddEntityFameworkValues(this IConfigurationBuilder builder, Action<EfConfgurationOptions> optionsAction = null)
        {
            //build the configuration up to this point
            var connectionStringConfig = builder.Build();

            //set the default settings
            var efOptions = new EfConfgurationOptions
            {
                ConnectionStringName = "DefaultConnection",
                PollingInterval = 1000
            };

            //invoke user customized settings
            optionsAction?.Invoke(efOptions);

            //configure entity framework to use SqlServer
            var dbOptions = new DbContextOptionsBuilder<ConfigurationContext>();
            var migrationsAssembly = typeof(EfConfigurationExtensions).GetTypeInfo().Assembly.GetName().Name;
            dbOptions = dbOptions.UseSqlServer(
                connectionStringConfig.GetConnectionString(efOptions.ConnectionStringName), 
                options => options.MigrationsAssembly(migrationsAssembly)
            );

            //add our new database source for configuration items
            return builder.Add(new EfConfigurationSource(dbOptions.Options, efOptions));
        }
    }
}