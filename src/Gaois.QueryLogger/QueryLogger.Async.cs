﻿using System;
using System.Threading.Tasks;
using Gaois.QueryLogger.Data;

#if NET461
    using System.Web;
#endif

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Logs query data to a data store asynchronously
    /// </summary>
    public static partial class QueryLogger
    {
        /// <summary>
        /// Logs query data to a data store asynchronously
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        /// <returns>The number of queries successfully logged</returns>
        public static async Task<int> LogAsync(string connectionString, params Query[] queries)
        {
            QueryLoggerSettings settings = new QueryLoggerSettings();
            return await LogAsync(connectionString, settings, queries);
        }

        /// <summary>
        /// Logs query data to a data store asynchronously
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <param name="settings">The <see cref="QueryLoggerSettings"/> to configure the logger with</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        /// <returns>The number of queries successfully logged</returns>
        public static async Task<int> LogAsync(string connectionString, QueryLoggerSettings settings, params Query[] queries)
        {
            foreach (Query query in queries)
            {
                string host = String.Empty;
                string ipAddress = String.Empty;

                #if NET461
                    var request = HttpContext.Current.Request;
                    host = request.Url.Host;
                    ipAddress = (String.IsNullOrEmpty(query.IPAddress)) ? request.UserHostAddress : query.IPAddress;
                #endif

                query.QueryID = (query.QueryID == null) ? Guid.NewGuid() : query.QueryID;
                query.Host = (String.IsNullOrEmpty(query.Host)) ? host : query.Host;
                query.IPAddress = IPAddressProcessor.Process(ipAddress, settings);
                query.LogDate = (query.LogDate == null) ? DateTime.UtcNow : query.LogDate;
            }

            try
            {
                return await LogStore.LogQueryAsync(connectionString, queries);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}