using System;

namespace EfConfiguration
{
    public class ConfigurationValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}