using System;

namespace Titanosoft.EfConfiguration
{
    public class ConfigurationValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        //This method is directly accessed by Entity Framework, users shouldn't set/change it
#pragma warning disable 649
        private DateTime _lastUpdated;
#pragma warning restore 649
        // ReSharper disable once ConvertToAutoProperty
        public DateTime LastUpdated => _lastUpdated;
    }
}