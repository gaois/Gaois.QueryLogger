using System;
using System.Threading.Tasks;
using Gaois.QueryLogger.Data;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Logs query data to a data store
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
            if (!_settings.CurrentValue.IsEnabled)
                return 0;

            foreach (var query in queries)
            {
                var host = _context.Request.Host.ToString();
                var ipAddress = (string.IsNullOrWhiteSpace(query.IPAddress)) 
                    ? _context.Connection.RemoteIpAddress.ToString() 
                    : query.IPAddress;

                query.ApplicationName = (string.IsNullOrWhiteSpace(query.ApplicationName))
                    ? _settings.CurrentValue.ApplicationName : query.ApplicationName;
                query.QueryID = (query.QueryID is null) ? Guid.NewGuid() : query.QueryID;
                query.Host = (string.IsNullOrWhiteSpace(query.Host)) ? host : query.Host;
                query.IPAddress = IPAddressProcessor.Process(ipAddress, _settings.CurrentValue);
                query.LogDate = (query.LogDate is null) ? DateTime.UtcNow : query.LogDate;
            }

            try
            {
                return await LogStore.LogQueryAsync(_connectionString, queries);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}