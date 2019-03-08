using Newtonsoft.Json;
using System;

namespace Gaois.QueryLogger
{
    public class AggregatedQueryLog
    {
        /// <summary>
        /// The name of the application that executes the query
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The application host domain
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The date and time of query logging. Gaois.QueryLogger logs this information automatically.
        /// </summary>
        public DateTime? LogDate { get; set; }

        /// <summary>
        /// The total number of aggregated queries
        /// </summary>
        public int TotalQueries { get; set; }

        /// <summary>
        /// The total number of aggregated unique queries. Useful in cases where mul
        /// </summary>
        public int TotalUniqueQueries { get; set; }

        public int ExecutedSuccessfully { get; set; }

        public int AverageExecutionTime { get; set; }

        /// <summary>
        /// Record additional data in JSON format
        /// </summary>
        public string JsonData { get; set; }

        /// <summary>
        /// Get a JSON representation for this aggregated query log
        /// </summary>
        public string ToJson() => JsonConvert.SerializeObject(this);

        /// <summary>
        /// Deserializes provided JSON into an AggregatedQueryLog object
        /// </summary>
        /// <param name="json">JSON representing an AggregatedQueryLog</param>
        /// <returns>The AggregatedQueryLog object</returns>
        public static AggregatedQueryLog FromJson(string json) =>
            JsonConvert.DeserializeObject<AggregatedQueryLog>(json);
    }
}