using System;
using System.Threading.Tasks;
using Gaois.QueryLogger.Data;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Logs query data to a data store asynchronously
    /// </summary>
    public partial class QueryLoggerCore : IQueryLogger
    {
        /// <summary>
        /// Logs query data to a data store asynchronously
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <returns>The number of queries successfully logged</returns>
        public async Task<int> LogAsync(params Query[] queries)
        {
            var context = Accessor.HttpContext;

            foreach (Query query in queries)
            {
                string host = context.Request.Host.ToString();
                string ipAddress = (String.IsNullOrEmpty(query.IPAddress)) ? context.Connection.RemoteIpAddress.ToString() : query.IPAddress;

                query.QueryID = (query.QueryID == null) ? Guid.NewGuid() : query.QueryID;
                query.Host = (String.IsNullOrEmpty(query.Host)) ? host : query.Host;
                query.IPAddress = IPAddressProcessor.Process(ipAddress, Settings.Value);
                query.LogDate = (query.LogDate == null) ? DateTime.UtcNow : query.LogDate;
            }

            try
            {
                return await LogStore.LogQueryAsync(Settings.Value.Store.ConnectionString, queries);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}