using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Titanosoft.EfConfiguration
{
    public class EfConfigurationSource : IConfigurationSource
    {
        private readonly DbContextOptions<ConfigurationContext> _dbOptions;
        private readonly EfConfgurationOptions _efOptions;

        /// <summary>
        /// Create a configuration source that returns configuration items from the database
        /// </summary>
        /// <param name="dbOptions">The Entity Framework Database Configuration</param>
        /// <param name="efOptions">The options to use for this source</param>
        public EfConfigurationSource(DbContextOptions<ConfigurationContext> dbOptions, EfConfgurationOptions efOptions)
        {
            _dbOptions = dbOptions;
            _efOptions = efOptions;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EfConfigurationProvider(_dbOptions, _efOptions);
        }
    }
}