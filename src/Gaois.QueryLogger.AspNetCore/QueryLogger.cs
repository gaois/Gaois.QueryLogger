using Ansa.Extensions;
using Gaois.QueryLogger.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Logs query data to a data store
    /// </summary>
    public partial class QueryLogger : IQueryLogger
    {
        private readonly IOptionsMonitor<QueryLoggerSettings> _settings;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpContext _context;
        private readonly SqlLogStore _store;

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        public QueryLogger(
            IOptionsMonitor<QueryLoggerSettings> settings,
            IHttpContextAccessor contextAccessor,
            SqlLogStore store)
        {
            _settings = settings;
            _contextAccessor = contextAccessor;
            _context = _contextAccessor.HttpContext;
            _store = store;
        }

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <returns>The number of queries successfully logged</returns>
        public void Log(params Query[] queries)
        {
            if (!_settings.CurrentValue.IsEnabled)
                return;

            foreach (var query in queries)
            {
                var host = _context.Request.Host.ToString();
                var ipAddress = (query.IPAddress.IsNullOrWhiteSpace())
                    ? _context.Connection.RemoteIpAddress.ToString()
                    : query.IPAddress;

                query.ApplicationName = (query.ApplicationName.IsNullOrWhiteSpace())
                    ? _settings.CurrentValue.ApplicationName : query.ApplicationName;
                query.QueryID = (query.QueryID is null) ? Guid.NewGuid() : query.QueryID;
                query.Host = (query.Host.IsNullOrWhiteSpace()) ? host : query.Host;
                query.IPAddress = IPAddressProcessor.Process(ipAddress, _settings.CurrentValue);
                query.LogDate = (query.LogDate is null) ? DateTime.UtcNow : query.LogDate;
            }

            _store.Enqueue(queries);
        }
    }
}