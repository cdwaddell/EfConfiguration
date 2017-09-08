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
            var connectionStringConfig = builder.Build();
            var efOptions = new EfConfgurationOptions
            {
                ConnectionStringName = "DefaultConnection",
                PollingInterval = 1000
            };
            optionsAction?.Invoke(efOptions);
            var dbOptions = new DbContextOptionsBuilder<ConfigurationContext>();
            var migrationsAssembly = typeof(EfConfigurationExtensions).GetTypeInfo().Assembly.GetName().Name;
            dbOptions.UseSqlServer(
                connectionStringConfig.GetConnectionString(efOptions.ConnectionStringName), 
                options => options.MigrationsAssembly(migrationsAssembly)
            );
            return builder.Add(new EfConfigurationSource(dbOptions.Options, efOptions));
        }
    }
}