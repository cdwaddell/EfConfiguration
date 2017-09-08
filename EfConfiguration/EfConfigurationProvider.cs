using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Titanosoft.EfConfiguration
{
    public class EfConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private static readonly object LockObject = new object();
        private static DateTime _lastRequested;

        public EfConfigurationProvider(DbContextOptions<ConfigurationContext> dbOptions, EfConfgurationOptions efOptions)
        {
            _cancellationToken = new CancellationTokenSource();
            _dbOptions = dbOptions;
            _efOptions = efOptions;
        }

        private readonly DbContextOptions<ConfigurationContext> _dbOptions;
        private readonly EfConfgurationOptions _efOptions;
        private readonly CancellationTokenSource _cancellationToken;
        private Task _backgroundWorker;

        public override void Load()
        {
            using (var dbContext = new ConfigurationContext(_dbOptions))
            {
                dbContext.Database.Migrate();

                _lastRequested = DateTime.UtcNow;
                Data = GetData(dbContext);
            }
            
            _backgroundWorker = Task.Factory.StartNew(token =>
            {
                while (!((CancellationToken)token).IsCancellationRequested)
                {
                    if (HasChanged)
                        UpdateDatabase();

                    Thread.Sleep(_efOptions.PollingInterval);
                }
            }, _cancellationToken.Token, _cancellationToken.Token);
        }
        
        private bool HasChanged
        {
            get
            {
                try
                {
                    using (var context = new ConfigurationContext(_dbOptions))
                    {
                        var now = DateTime.UtcNow;
                        var lastUpdated = context.Values
                            .Where(c => c.LastUpdated <= now)
                            .OrderByDescending(v => v.LastUpdated)
                            .Select(v => v.LastUpdated)
                            .FirstOrDefault();

                        var hasChanged = lastUpdated > _lastRequested;

                        _lastRequested = lastUpdated;

                        return hasChanged;
                    }
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        private void UpdateDatabase()
        {
            using (var dbContext = new ConfigurationContext(_dbOptions))
            {
                var dict = GetData(dbContext);
                lock (LockObject)
                {
                    Data = dict;
                }
                OnReload();
            }
        }
        
        private static IDictionary<string, string> GetData(ConfigurationContext dbContext)
        {
            try
            {
                return dbContext.Values.ToDictionary(c => c.Key, c => c.Value);
            }
            catch (SqlException)
            {
                return new Dictionary<string, string>();
            }
        }

        public void Dispose()
        {
            _cancellationToken.Cancel();
            _backgroundWorker?.Dispose();
            _backgroundWorker = null;
        }
    }
}
