using System;
using System.Threading.Tasks;
using Gaois.QueryLogger.Data;

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
            foreach (Query query in queries)
            {
                query.IPAddress = IPAddressProcessor.Process(query.IPAddress, settings);
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
