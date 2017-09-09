namespace Titanosoft.EfConfiguration
{
    /// <summary>
    /// This class is used to configure the EfConfiguration module
    /// </summary>
    public class EfConfgurationOptions
    {
        /// <summary>
        /// Get or Set the name of the connection string to your database, defaults to "<value>DefaultConnection</value>"
        /// </summary>
        public string ConnectionStringName { get; set; }
        /// <summary>
        /// Get or Set the interval, in milliseconds, between attempts to check the database for updates, defaults to 1000
        /// </summary>
        public int PollingInterval { get; set; }
    }
}