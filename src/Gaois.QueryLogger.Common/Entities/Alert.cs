using System;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Provides information about a possible issue with the query logger service
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// The alert type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Query metadata
        /// </summary>
        public Query Query { get; set; }
        
        /// <summary>
        /// Represents an error that has occurred
        /// </summary>
        public Exception Exception { get; set; }
    }
}