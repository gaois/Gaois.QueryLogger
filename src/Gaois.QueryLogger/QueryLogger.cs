using System;
using System.Threading.Tasks;
using Gaois.QueryLogger.Data;

#if NET461
    using System.Web;
#endif

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Logs query data to a data store
    /// </summary>
    public static partial class QueryLogger
    {
        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        /// <returns>The number of queries successfully logged</returns>
        public static int Log(string connectionString, params Query[] queries)
        {
            var settings = new QueryLoggerSettings();
            return Log(connectionString, settings, queries);
        }

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <param name="settings">The <see cref="QueryLoggerSettings"/> to configure the logger with</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        /// <returns>The number of queries successfully logged</returns>
        public static int Log(string connectionString, QueryLoggerSettings settings, params Query[] queries)
        {
            if (!settings.IsEnabled)
                return 0;

            foreach (var query in queries)
            {
                var host = default(string);
                var ipAddress = default(string);

                #if NET461
                    var request = HttpContext.Current.Request;
                    host = request.Url.Host;
                    ipAddress = (string.IsNullOrWhiteSpace(query.IPAddress))
                        ? request.UserHostAddress : query.IPAddress;
                #endif

                query.ApplicationName = (string.IsNullOrWhiteSpace(query.ApplicationName)) 
                    ? settings.ApplicationName : query.ApplicationName;
                query.QueryID = (query.QueryID is null) ? Guid.NewGuid() : query.QueryID;
                query.Host = (string.IsNullOrWhiteSpace(query.Host)) ? host : query.Host;
                query.IPAddress = IPAddressProcessor.Process(ipAddress, settings);
                query.LogDate = (query.LogDate is null) ? DateTime.UtcNow : query.LogDate;
            }

            try
            {
                return LogStore.LogQuery(connectionString, queries);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}