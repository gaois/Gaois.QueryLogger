using System;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Data that describe the query to be logged
    /// </summary>
    public class Query
    {
        /// <summary>
        /// A unique ID that represents a specific query or group of queries
        /// </summary>
        public Guid QueryID { get; set; }

        /// <summary>
        /// The name of the application that executes the query
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Relates the query to a category specified by the application
        /// </summary>
        public string QueryCategory { get; set; }

        /// <summary>
        /// The query term(s) or text content
        /// </summary>
        public string QueryText { get; set; }

        /// <summary>
        /// The application host domain
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The client IP address
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Records whether the query executed successfully. The default value is true.
        /// </summary>
        public bool ExecutedSuccessfully { get; set; } = true;
        
        /// <summary>
        /// The query execution time in milliseconds
        /// </summary>
        public int? ExecutionTime { get; set; }

        /// <summary>
        /// The count of results returned by the query.
        /// </summary>
        public int? ResultCount { get; set; }

        /// <summary>
        /// The date and time of query logging. Gaois.QueryLogger logs this information automatically.
        /// </summary>
        public DateTime? LogDate { get; set; }

        /// <summary>
        /// Record additional data in JSON format
        /// </summary>
        public string JsonData { get; set; }
    }
}