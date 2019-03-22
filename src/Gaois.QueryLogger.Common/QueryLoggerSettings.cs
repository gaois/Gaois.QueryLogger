namespace Gaois.QueryLogger
{
    /// <summary>
    /// Specifies settings that configure the query logger
    /// </summary>
    public class QueryLoggerSettings
    {
        /// <summary>
        /// Specifies a global name for your application that can used in all queries logged
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Specifies whether the application is configured to log queries. The default value is true.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Specifies settings that configure the query logger store
        /// </summary>
        public QueryLoggerStoreSettings Store { get; set; } = new QueryLoggerStoreSettings();

        /// <summary>
        /// Specifies whether the client IP address should be logged. The default value is true.
        /// </summary>
        public bool StoreClientIPAddress { get; set; } = true;

        /// <summary>
        /// Sets the level of client IP address anonymization. Defaults to partial anonymization.
        /// </summary>
        public IPAddressAnonymizationLevel AnonymizeIPAddress { get; set; } = IPAddressAnonymizationLevel.Partial;
    }
}