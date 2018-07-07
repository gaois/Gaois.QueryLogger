using System;
using Microsoft.AspNetCore.Http;
using Gaois.QueryLogger.Data;

namespace Gaois.QueryLogger.AspNetCore
{
    /// <summary>
    /// Logs query data to a data store
    /// </summary>
    public static partial class QueryLogger
    {
        /*public static void Log(params Query[] queries)
        {
            string connectionString = ConfigurationAccess.GetConnectionString();
            QueryLoggerSettings settings = new QueryLoggerSettings();
            Log(connectionString, settings, queries);
        }

        public static void Log(QueryLoggerSettings settings, params Query[] queries)
        {
            string connectionString = ConfigurationAccess.GetConnectionString();
            Log(connectionString, settings, queries);
        }*/

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        public static void Log(string connectionString, params Query[] queries)
        {
            QueryLoggerSettings settings = new QueryLoggerSettings();
            Log(connectionString, settings, queries);
        }

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <param name="settings">The <see cref="QueryLoggerSettings"/> to configure the logger with</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        public static void Log(string connectionString, QueryLoggerSettings settings, params Query[] queries)
        {
            var request = HttpContext.Current.Request;

            foreach (Query query in queries)
            {
                string host = Request.Url.Host;
                string ipAddress = String.Empty;

                host = request.Url.Host;
                ipAddress = (String.IsNullOrEmpty(query.IPAddress)) ? request.UserHostAddress : query.IPAddress;

                query.Host = (String.IsNullOrEmpty(query.Host)) ? host : query.Host;
                query.IPAddress = IPAddressProcessor.Process(ipAddress, settings);
                query.LogDate = (query.LogDate == null) ? DateTime.UtcNow : query.LogDate;
            }

            try
            {
                LogStore.LogQuery(connectionString, queries);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
