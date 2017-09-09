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

        /// <inheritdoc />
        /// <summary>
        /// The initial load for the Data dictionary, this also configures the background worker
        /// </summary>
        public override void Load()
        {
            using (var dbContext = new ConfigurationContext(_dbOptions))
            {
                try
                {
                    dbContext.Database.Migrate();
                }
                catch (InvalidOperationException)
                {
                    dbContext.Database.EnsureCreated();
                }

                _lastRequested = DateTime.UtcNow;
                Data = GetDictionaryFromDatabase(dbContext);
            }
            
            _backgroundWorker = Task.Factory.StartNew(token =>
            {
                while (!((CancellationToken)token).IsCancellationRequested)
                {
                    if (HasChanged) UpdateFromDatabase();

                    Thread.Sleep(_efOptions.PollingInterval);
                }
            }, _cancellationToken.Token, _cancellationToken.Token);
        }
        
        /// <summary>
        /// Flag the values as changed if the lastupdated date of an item is after the last run
        /// </summary>
        private bool HasChanged
        {
            get
            {
                try
                {
                    using (var context = new ConfigurationContext(_dbOptions))
                    {
                        var now = DateTime.UtcNow;

                        //Query the database as quickly as you can to determine if anything has been updated
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

        /// <summary>
        /// Query the database and update the dictionary in a thread safe way
        /// </summary>
        private void UpdateFromDatabase()
        {
            using (var dbContext = new ConfigurationContext(_dbOptions))
            {
                var dict = GetDictionaryFromDatabase(dbContext);
                lock (LockObject)
                {
                    Data = dict;
                }
                OnReload();
            }
        }
        
        /// <summary>
        /// Retreives the configuration dictionary from the database
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns>The KeyValuePairs that represent configuration entries from the database</returns>
        private static IDictionary<string, string> GetDictionaryFromDatabase(ConfigurationContext dbContext)
        {
            try
            {
                return dbContext.Values
                    .ToDictionary(c => c.Key, c => c.Value);
            }
            catch (SqlException)
            {
                return new Dictionary<string, string>();
            }
        }

        public void Dispose()
        {
            //since we have a background thread, we need to stop it
            _cancellationToken.Cancel();
            //then dispose of it
            _backgroundWorker?.Dispose();
            _backgroundWorker = null;
        }
    }
}
