using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Titanosoft.EfConfiguration;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EfConfiguration.Tests
{
    public class EfConfigurationExtensionsTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ThrowsIfNoConnectionString()
        {
            Assert.Throws(typeof(ArgumentNullException), () =>
                new ConfigurationBuilder().AddEntityFameworkValues()
            );
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void DoesNotThrowIfConnectionString()
        {
            var expected = new KeyValuePair<string, string>("ConnectionStrings:DefaultConnection", "SomeConnectionString");

            new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>> {expected})
                .AddEntityFameworkValues();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void DoesNotThrowIfCustomConnectionString()
        {
            const string name = "SomeConnectionName";
            var expected = new KeyValuePair<string, string>($"ConnectionStrings:{name}", "SomeConnectionString");

            new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>> {expected})
                .AddEntityFameworkValues(options => options.ConnectionStringName = name);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ValueGetsUpdated()
        {
            var dbOptions = new DbContextOptionsBuilder<ConfigurationContext>()
                .UseInMemoryDatabase("TestValues");

            var initial = "Initial";
            var expected = "Expected";
            var key = "Test:Test";
            var timeout = 500;

            using (var context = new ConfigurationContext(dbOptions.Options))
            {
                context.Values.Add(new ConfigurationValue {Key = "Test:Test", Value = initial});
                context.SaveChanges();
            }

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>> { new KeyValuePair<string,string>(key, expected) })
                .TestAddEntityFameworkValues(dbOptions, timeout)
                .Build();

            var actual = configuration[key];
            Assert.Equal(initial, actual);
            
            using (var context = new ConfigurationContext(dbOptions.Options))
            {
                context.Values.Single(x => x.Key == "Test:Test").Value = expected;
                context.SaveChanges();
            }

            Thread.Sleep(timeout);

            var newActual = configuration[key];
            Assert.Equal(initial, newActual);
        }
    }

    internal static class ExtensionFeatures
    {
        internal static IConfigurationBuilder TestAddEntityFameworkValues(this IConfigurationBuilder builder, DbContextOptionsBuilder<ConfigurationContext> dbOptions, int pollingInterval = 1000)
        {
            //set the default settings
            var efOptions = new EfConfgurationOptions
            {
                ConnectionStringName = "DefaultConnection",
                PollingInterval = pollingInterval
            };

            //add our new database source for configuration items
            return builder.Add(new EfConfigurationSource(dbOptions.Options, efOptions));
        }
    }
}
