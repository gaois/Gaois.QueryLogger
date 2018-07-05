using System;
using System.Web;
using Gaois.QueryLogger.Data;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Logs query data to a data store
    /// </summary>
    public static class QueryLogger
    {
        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="query">The <see cref="Query"/> object to be logged</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        public static void Log(Query query, string connectionString)
        {
            QueryLoggerSettings settings = new QueryLoggerSettings();
            Log(query, settings, connectionString);
        }

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="query">The <see cref="Query"/> object to be logged</param>
        /// <param name="settings">The <see cref="QueryLoggerSettings"/> to configure the logger with</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        public static void Log(Query query, QueryLoggerSettings settings, string connectionString)
        {
            query.IPAddress = ProcessIPAddress(query.IPAddress, settings);
            query.LogDate = DateTime.UtcNow;

            try
            {
                LogStore.LogQuery(query, connectionString);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static string ProcessIPAddress(string ip, QueryLoggerSettings settings)
        { 
            if (settings.StoreClientIPAddress == false)
            {
                return "PRIVATE";
            }

            if (String.IsNullOrEmpty(ip)) 
            {
                return "UNKNOWN";
            }

            string result;

            switch (settings.AnonymizeIPAddress)
            {
                case IPAddressAnonymizationLevel.None:
                    result = ip;
                    break;
                case IPAddressAnonymizationLevel.Partial:
                    result = PartiallyAnonymizeIP(ip);
                    break;
                default:
                    result = String.Empty;
                    break;
            }

            return result;
        }

        private static string PartiallyAnonymizeIP(string ip)
        {
            int lastPosition = ip.LastIndexOf(".");
            return (lastPosition > 0) ? ip.Substring(0, lastPosition) : ip;
        }
    }
}
