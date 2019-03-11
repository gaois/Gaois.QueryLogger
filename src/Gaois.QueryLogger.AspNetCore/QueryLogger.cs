using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Gaois.QueryLogger.Data;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Logs query data to a data store
    /// </summary>
    public partial class QueryLoggerCore : IQueryLogger
    {
        private readonly IOptionsMonitor<QueryLoggerSettings> _settings;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _connectionString;
        private readonly HttpContext _context;

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        public QueryLoggerCore(
            IOptionsMonitor<QueryLoggerSettings> settings,
            IHttpContextAccessor contextAccessor)
        {
            _settings = settings;
            _contextAccessor = contextAccessor;
            _connectionString = _settings.CurrentValue.Store.ConnectionString;
            _context = _contextAccessor.HttpContext;
        }

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <returns>The number of queries successfully logged</returns>
        public int Log(params Query[] queries)
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
                return LogStore.LogQuery(_connectionString, queries);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}