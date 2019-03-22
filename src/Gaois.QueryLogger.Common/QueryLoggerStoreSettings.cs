namespace Gaois.QueryLogger
{
    /// <summary>
    /// Specifies settings that configure the query logger store
    /// </summary>
    public class QueryLoggerStoreSettings
    {
        /// <summary>
        /// The connection string for a data store
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The name of a connection string specified in the ConnectionStrings configuration
        /// </summary>
        public string ConnectionStringName { get; set; }

        /// <summary>
        /// The maximum amount of time queries will await enqueuing before being discarded
        /// </summary>
        public int MaxQueueRetryTime { get; set; } = 30;

        /// <summary>
        /// The maximum possible size of the query log queue before new entries will be blocked
        /// </summary>
        public int MaxQueueSize { get; set; } = 1000;

        /// <summary>
        /// The table name (optionally including schema), e.g. "dbo.QueryLogs" to use when logging queries. The default value is "QueryLogs".
        /// </summary>
        public string TableName { get; set; } = "QueryLogs";
    }
}