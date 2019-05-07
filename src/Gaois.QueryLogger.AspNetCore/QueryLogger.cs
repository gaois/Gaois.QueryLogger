using Ansa.Extensions;
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
        private readonly IOptionsSnapshot<QueryLoggerSettings> _settings;
        private readonly ILogStore _store;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpContext _context;

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        public QueryLogger(
            IOptionsSnapshot<QueryLoggerSettings> settings,
            ILogStore store,
            IHttpContextAccessor contextAccessor)
        {
            _settings = settings;
            _store = store;
            _contextAccessor = contextAccessor;
            _context = _contextAccessor.HttpContext;
        }

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <returns>The number of queries successfully logged</returns>
        public void Log(params Query[] queries)
        {
            if (!_settings.Value.IsEnabled)
                return;

            foreach (var query in queries)
            {
                var host = _context.Request.Host.ToString();
                var ipAddress = (query.IPAddress.IsNullOrWhiteSpace())
                    ? _context.Connection.RemoteIpAddress.ToString()
                    : query.IPAddress;

                query.ApplicationName = (query.ApplicationName.IsNullOrWhiteSpace())
                    ? _settings.Value.ApplicationName : query.ApplicationName;
                query.QueryID = (query.QueryID is null) ? Guid.NewGuid() : query.QueryID;
                query.Host = (query.Host.IsNullOrWhiteSpace()) ? host : query.Host;
                query.IPAddress = IPAddressProcessor.Process(ipAddress, _settings.Value);
                query.LogDate = (query.LogDate is null) ? DateTime.UtcNow : query.LogDate;
            }

            _store.Enqueue(queries);
            _store.ProcessQueue();
        }
    }
}