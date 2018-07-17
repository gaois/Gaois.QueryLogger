namespace Gaois.QueryLogger
{
    /// <summary>
    /// Specifies settings that configure the query logger
    /// </summary>
    public class QueryLoggerSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Specifies whether the client IP address should be logged. The default value is true.
        /// </summary>
        public bool StoreClientIPAddress = true;

        /// <summary>
        /// Sets the level of client IP address anonymization. Defaults to partial anonymization.
        /// </summary>
        public IPAddressAnonymizationLevel AnonymizeIPAddress = IPAddressAnonymizationLevel.Partial;
    }
}