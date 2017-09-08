using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Titanosoft.EfConfiguration
{
    public class EfConfigurationSource : IConfigurationSource
    {
        private readonly DbContextOptions<ConfigurationContext> _dbOptions;
        private readonly EfConfgurationOptions _efOptions;

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